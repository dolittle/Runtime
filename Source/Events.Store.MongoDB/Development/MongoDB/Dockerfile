FROM mongo:4.2.2-bionic

COPY replicaset-initiate.js /docker-entrypoint-initdb.d/

COPY mongo-backup.sh /mongo-backup.sh

CMD ["mongod", "--replSet", "rs0"]