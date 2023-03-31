#!/bin/sh

usage() {
    cat <<EOM
    Description: Removes 

    Usage:
    $(basename $0) <mongo_host> <archive_location> [<backup_filename_prefix>] [<days_of_history_to_keep>]

    Example:
    $(basename $0) mongo.application-0454744b-3fdc-4c49-bce6-d47b63b9ed3d.svc.cluster.local /mnt/backup monday-dev 100

EOM
    exit 0
}

[ -z $1 ] && { usage; }

DEFAULT_DAYS_OF_HISTORY_TO_KEEP=9000
DEFAULT_BACKUP_FILENAME_PREFIX="mongo-backup-"

mongo_host=$1
archive_location=$2
backup_filename_prefix=$3
days_of_history_to_keep=$4

if [ -z "$mongo_host" ] || [ -z "$archive_location" ]; then
  echo 'Please make sure mongo_host and archive_location parameters are provided'        
  exit 1
fi

if [ ! -d "$archive_location" ]; then
    echo "Error: Directory $archive_location does not exists."
    exit 1
fi

if [ -z "$backup_filename_prefix" ] ; then
	echo "No value for backup_filename_prefix parameter is provided. Using the default value: $DEFAULT_BACKUP_FILENAME_PREFIX"
	backup_filename_prefix=$DEFAULT_BACKUP_FILENAME_PREFIX
fi

if [ -z "$days_of_history_to_keep" ] ; then
	echo "No value for days_of_history_to_keep parameter is provided. Using the default value: $DEFAULT_DAYS_OF_HISTORY_TO_KEEP"
	days_of_history_to_keep=$DEFAULT_DAYS_OF_HISTORY_TO_KEEP
fi

files_to_remove_count=$(find $archive_location -type f -mtime +$days_of_history_to_keep | wc -l)

echo "Found $files_to_remove_count files older than $days_of_history_to_keep in $archive_location. Deleting"

find $archive_location -type f -mtime +$days_of_history_to_keep -delete

mongo --host=$mongo_host --eval "db.stats()" > /dev/null

RESULT=$?   # returns 0 if mongo eval succeeds

if [ $RESULT -ne 0 ]; then
    echo "Not able to connect to mongodb"
    exit 1
fi

mongodump_file="$archive_location/$backup_filename_prefix-$(date +%Y-%m-%d_%H-%M-%S).gz.mongodump"

echo "Writing mongo backup to $mongodump_file"

mongodump --host=$mongo_host --gzip --archive=$mongodump_file


echo "Verifying the backup file $mongodump_file"

mongorestore --gzip --archive=$mongodump_file  --dryRun --objcheck

RESTORE_RESULT=$?

if [ $RESTORE_RESULT -ne 0 ]; then
    echo "Error verifying mongo dump file: mongorestore dry run failed"
    exit 1
else
    echo "Mongo backup job completed successfully"
fi