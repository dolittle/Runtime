FROM mongo:4.2.2-bionic

COPY replicaset-initiate.js /docker-entrypoint-initdb.d/

CMD ["mongod", "--replSet", "rs0"]