import json
import os
import pandas as pd
import dotenv
from confluent_kafka import Consumer
import redis
import geopandas as gpd
from dotenv import load_dotenv
from shapely.geometry import Point
import logging
from datetime import datetime, timezone
import sys
from elasticsearch import Elasticsearch
import pika

class Elastic8Handler(logging.Handler):
    def __init__(self, client, index_name):
        super().__init__()
        self.client = client
        self.index_name = index_name

    def emit(self, record):
        if record.name.startswith(('elastic', 'urllib3')):
            return
        try:
            log_entry = self.format(record)
            doc = {
                'timestamp': datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%S.%fZ'),
                'level': record.levelname,
                'message': log_entry,
                'logger': record.name
            }
            self.client.index(index=self.index_name, document=doc)
        except Exception as e:
            print(f"ELASTIC REJECTED LOG: {e}")


os.makedirs('/../logs', exist_ok=True)

logging.basicConfig(level=logging.INFO,
                    format='%(asctime)s | %(levelname)s | %(message)s',
                    datefmt='%Y-%m-%d %H:%M:%S',
                    handlers=[
                        logging.FileHandler('/../logs/log_file.log'),
                        logging.StreamHandler(sys.stdout)
                    ])

load_dotenv()
logger = logging.getLogger()
es_client = Elasticsearch(os.getenv("ELASTIC_URI","http://localhost:9200"))

es_handler = Elastic8Handler(es_client, "python-consumer-logs")
logger.addHandler(es_handler)


def get_rabbit_channel():
    credentials = pika.PlainCredentials(os.getenv('RABBIT_CRED', 'root'), os.getenv('RABBIT_CRED', 'root'))
    parameters = pika.ConnectionParameters(
        host=os.getenv('RABBIT_URI', 'localhost'),
        credentials=credentials
    )
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()
    channel.exchange_declare(
        exchange='direct_alerts',
        exchange_type='direct')
    channel.queue_declare(queue='north',
                          durable=True,
                          auto_delete=False,
                          exclusive=False)
    channel.queue_declare(queue='center',
                          durable=True,
                          auto_delete=False,
                          exclusive=False)
    channel.queue_declare(queue='south',
                          durable=True,
                          auto_delete=False,
                          exclusive=False)
    channel.queue_declare(queue='overseas',
                          durable=True,
                          auto_delete=False,
                          exclusive=False)

    channel.queue_bind(exchange="direct_alerts",
                       queue='north',
                       routing_key='NORTH')
    channel.queue_bind(exchange="direct_alerts",
                       queue='center',
                       routing_key='CENTER')
    channel.queue_bind(exchange="direct_alerts",
                       queue='south',
                       routing_key='SOUTH')
    channel.queue_bind(exchange="direct_alerts",
                       queue='overseas',
                       routing_key='OVERSEAS')

    return channel

def main():
    conf = {
        'bootstrap.servers': os.getenv('KAFKA_URI', 'localhost:9092'),
        'group.id': 'foo',
        'auto.offset.reset': 'earliest'}
    consumer = None

    redis_server = redis.Redis(host=os.getenv('REDIS_URI', 'localhost'),
                               port=os.getenv('REDIS_PORT', 6379), decode_responses=True)

    channel = get_rabbit_channel()

    try:
        consumer = Consumer(conf)
        consumer.subscribe(["alerts-topic"])

        while True:
            msg = consumer.poll(1.0)
            if msg is None:
                continue
            elif msg.error():

                logger.error(f"ERROR: {msg.error()}")
            else:
                alert_process(msg.value(), redis_server, channel)
    except Exception as ex:
        print(ex)
        logger.error(f"Error while consuming message {ex}")
    finally:
        consumer.close()


def alert_process(msg, redis_server, channel):
    try:
        json_msg = json.loads(msg)
        is_in_redis = redis_server.get(json_msg["alert_id"])
        if is_in_redis is None:
            if json_msg["alert_id"]!= "pikud-haoref":
                redis_server.set(json_msg["alert_id"], json.dumps(json_msg), ex=86000)
            else:
                redis_server.set(json_msg["alert_id"], json.dumps(json_msg), ex=36000)
        else:
            if is_in_redis["source"] != "pikud-haoref" and redis_server.tll(json_msg["alert_id"]) <= 3600:
                redis_server.set(json_msg["alert_id"], json.dumps(json_msg), ex=86000)
            if is_in_redis["source"] == "pikud-haoref" and redis_server.tll(json_msg["alert_id"]) <= 30000:
                redis_server.set(json_msg["alert_id"], json.dumps(json_msg), ex=36000)

        if validate_alert(json_msg):
            classification = classify_msg(json_msg)
            print("classification:", classification)
            channel.basic_publish(exchange='direct_alerts',
                                  routing_key=classification,
                                  body=json.dumps(json_msg))
            print(json.dumps(json_msg), classification)
            logger.info(f"Sent {json_msg['alert_id']} to Rabbit Exchange with classification: {classification}.")

    except Exception as ex:
        print(ex)
        logger.error(f"Error while processing message {ex}")



def validate_alert(json_msg):
    if json_msg["source"] in [None, ""] or json_msg["priority"] in [None, ""] or json_msg["classification"] in [None, ""] or json_msg["lat"] in [None, ""] or json_msg["lon"] in [None, ""] or json_msg["timestamp"] in [None, ""] or json_msg["status"] in [None, ""]:
        logger.error(f"alert cant have empty feilds")
        return  False

    if json_msg["source"] not in ['aman', 'mossad', 'pikud-haoref', 'shabak']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid source")
        return False
    if json_msg['source'] == 'aman' and json_msg['title'] not in ["זוהה כלי טיס בלתי מאויש עוין" ,'זוהו הכנות לשיגור','זוהה שיגור טיל בליסטי','שיבושי ניווט באזור','תנועת כוחות חריגה סמוך לגבול','זוהה שיגור רקטות']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid source")
        return False
    if json_msg['source'] == 'mossad' and json_msg['title'] not in ["התרעה על כוונה לפגוע ביעד ישראלי בחוץ לארץ", 'זוהה נתיב הברחת אמצעי לחימה',
                                                                   'פעילות חריגה באתר אסטרטגי', 'ניסיון כניסה של פעיל עוין לישראל',
                                                                   'העברת כספים לארגון טרור',
                                                                   'תנועת פעיל עוין בין מדינות']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid source")
        return False
    if json_msg['source'] == 'pikud-haoref' and json_msg['title'] not in ["ירי רקטות וטילים" ,'חדירת כלי טיס עוין','חדירת מחבלים','התרעה מקדימה','רעידת אדמה','האירוע הסתיים']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid source")
        return False
    if json_msg['source'] == 'shabak' and json_msg['title'] not in ["התרעה חמה לפיגוע" ,'תנועת מחבל מבוקש','חשד לחדירה ליישוב','רכב חשוד','חשד לפעילות ריגול עבור גורם עוין','גניבת אמצעי לחימה']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid source")
        return False

    if json_msg['priority'] not in ['CRITICAL', 'HIGH', 'MEDIUM', 'LOW']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid priority")
        return False

    if json_msg['classification'] not in ['UNCLASSIFIED', 'RESTRICTED', 'SECRET', 'TOP_SECRET']:
        logger.error(f"alert {json_msg["alert_id"]} has un valid classification")
        return False

    if json_msg['lat'] < -90 or json_msg['lat'] > 90:
        logger.error(f"alert {json_msg["alert_id"]} has out of range lat")
        return False
    if json_msg['lon'] < -180 or json_msg['lon'] > 180:
        logger.error(f"alert {json_msg["alert_id"]} has out of range lon")
        return False
    if json_msg['status'] != 'WAITING':
         logger.error(f"alert {json_msg["alert_id"]} has out un valid status")
         return False
    return True


def classify_msg(json_msg):
    return get_region_with_geopandas('regions.geojson', json_msg['lon'], json_msg['lat']);


def get_region_with_geopandas(file_path: str, lon: float, lat: float) -> str:
    gdf = gpd.read_file(file_path)

    pt = Point(lon, lat)

    matched = gdf[gdf.geometry.contains(pt)]

    if not matched.empty:
        return matched.iloc[0]["region"]
    return "OVERSEAS"


if __name__ == "__main__":
    main()
