FINAL EXAM PIPLINE:

PART 1:
runing a background service on the files, reading from each file that has a *.ready brother.
then deleteing the .ready file and reading the .json /.txt alert file - and sending in to kafka topic.
the design has DI and all solid concepts.
and logs are sent to elastic for later on analizing

PART 2:
a python consumer that read from the kafka topic, validates, get command and sends to rabbit queue and elastic.
    the validations:
        chack for each feild that it is not missing, and that the feilds that have a certien value are realy one of the alawed values.
    then check reddis:
        if does not exist or: for pikud horef a few is there for a few ahors alraedy then add the new on, and for oth senders after a day that it is there (diferent TLL's)
    then calculate command center:
        function that we got that get point and calculates command center.
    then:
        send to rabbit exchange which will decide to whice queue to send to.
        and also log it in elastic and make a kibana dashboard to show how many messages per command every momant

Part 3:
a c# rabbit consumer how reads from rabbit queues and then validates each message again (source of truth),
then opens a sql scope and adds to the specific tables per command.
(I gave every table its own model althogh thay are the same - becuse the migration built only 1 table
if all of them had the same model).
I decided to se a relational db like mysql becuse its structured and every alert should have the same fuilds
and i dont want a document like mongoDb which we are not commited to 1  structure.
also - the hebrew data un sql is a bunch of question marks, but when u read it - it is in goot format.

You must run part 3 befaur 2 becuse only part 2 declares the rabbit queues and binds them.

the sql key is a row number and not alert id, cuz i alow duplicates if anough time passed.

Part 4:
reads from mysql and checks if we have and other commands that got the same message in a period of 5 hors,
if yes send critical log, then in kibana show dashboard of critical logs 
aftr that we send the alertt to be processed, each kind gets a different process time to simulate a real processing.

Part 5:
API that can get data and summerys out of the mysql db
(I know the last endpont is writtin like crap but my wife is waiting...  :))

The code runs:
part 1 -> part 3 -> part 2 -> part 4 and part 5
