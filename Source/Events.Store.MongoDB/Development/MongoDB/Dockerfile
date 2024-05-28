FROM mongo:7.0

COPY replicaset-initiate.js /docker-entrypoint-initdb.d/

CMD ["mongod", "--replSet", "rs0"]
