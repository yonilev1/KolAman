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
