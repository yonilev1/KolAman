import json
import os
import pandas as pd
import dotenv
from confluent_kafka import Consumer
import redis
import geopandas as gpd
from shapely.geometry import Point
import logging
from datetime import datetime, timezone
import sys
from elasticsearch import Elasticsearch


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

logger = logging.getLogger()
es_client = Elasticsearch("http://elastic:9200")

es_handler = Elastic8Handler(es_client, "python-consumer-logs")
logger.addHandler(es_handler)

def main():
    conf = {
        'bootstrap.servers': 'localhost:9092',
        'group.id': 'foo',
        'auto.offset.reset': 'earliest'}


    consumer = None

    redis_server = redis.Redis(host='localhost', port=6379)

    try:
        consumer = Consumer(conf)
        consumer.subscribe(["alerts-topic"])

        while True:
            msg = consumer.poll(1.0)
            if msg is None:
                continue
            if msg.error():
                logger.error(f"ERROR: {msg.error()}")
            else:
                alert_process(msg.value(), redis_server)
    except Exception as ex:
        print(ex)
        logger.error(f"Error while consuming message {ex}")
    finally:
        consumer.close()


def alert_process(msg, redis_server):
    json_msg = json.loads(msg)
    is_in_redis = redis_server.get(json_msg["alert_id"])
    if (is_in_redis is None
            or (is_in_redis["source"] != "pikud-haoref" and is_in_redis.get("ex", 0) <= 18000)):
        redis_server.set(json_msg["alert_id"], ex=36000)
    if is_in_redis["source"] == "pikud-haoref" and is_in_redis.get("ex", 0) <= 30000:
        redis_server.set(json_msg["alert_id"], ex=36000)
    """Log"""

    if validate_alert(json_msg):
        classification = classify_msg(json_msg)
        print(json_msg, classification)


def validate_alert(json_msg):
    if json_msg["source"] in [None, ""] or json_msg["priority"] in [None, ""] or json_msg["classification"] in [None, ""] or json_msg["lat"] in [None, ""] or json_msg["lon"] in [None, ""] or json_msg["timestamp"] in [None, ""] or json_msg["status"] in [None, ""]:
        return  False

    if json_msg["source"] not in ['aman', 'mossad', 'pikud-haoref', 'shabak']:
        """Log"""
        return False
    if json_msg['source'] == 'aman' and json_msg['source'] not in ["זוהה כלי טיס בלתי מאויש עוין" ,'זוהו הכנות לשיגור','זוהה שיגור טיל בליסטי','שיבושי ניווט באזור','תנועת כוחות חריגה סמוך לגבול','זוהה שיגור רקטות']:
        """log"""
        return False
    if json_msg['source'] == 'mossad' and json_msg['source'] not in ["התרעה על כוונה לפגוע ביעד ישראלי בחוץ לארץ", 'זוהה נתיב הברחת אמצעי לחימה',
                                                                   'פעילות חריגה באתר אסטרטגי', 'ניסיון כניסה של פעיל עוין לישראל',
                                                                   'העברת כספים לארגון טרור',
                                                                   'תנועת פעיל עוין בין מדינות']:
        """log"""
        return False
    if json_msg['source'] == 'pikud-haoref' and json_msg['source'] not in ["ירי רקטות וטילים" ,'חדירת כלי טיס עוין','חדירת מחבלים','התרעה מקדימה','רעידת אדמה','האירוע הסתיים']:
        """log"""
        return False
    if json_msg['source'] == 'shabak' and json_msg['source'] not in ["התרעה חמה לפיגוע" ,'תנועת מחבל מבוקש','חשד לחדירה ליישוב','רכב חשוד','חשד לפעילות ריגול עבור גורם עוין','גניבת אמצעי לחימה']:
        """log"""
        return False

    if json_msg['priority'] not in ['CRITICAL', 'HIGH', 'MEDIUM', 'LOW']:
        """log"""
        return False

    if json_msg['classification'] not in ['UNCLASSIFIED', 'RESTRICTED', 'SECRET', 'TOP_SECRET']:
        """log"""
        return False

    if json_msg['lat'] < -90 or json_msg['lat'] > 90:
        """log"""
        return False
    if json_msg['lon'] < -180 or json_msg['lon'] > 180:
        """log"""
        return False
    if json_msg['status'] != 'WAITING':
        """log"""
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
