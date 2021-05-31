#!/bin/bash
# bootstrap clam av service and clam av database updater shell script
set -m

# Init AV Db
freshclam
# start clam service itself and the updater in background as daemon
freshclam -d &
clamd &
(cd ./RestProxy && exec ./ClamAvProxy) & 

# recognize PIDs
pidlist=$(jobs -p)

# initialize latest result var
latest_exit=0

# define shutdown helper
function shutdown() {
    trap "" SIGINT

    for single in $pidlist; do
        if ! kill -0 "$single" 2> /dev/null; then
            wait "$single"
            latest_exit=$?
        fi
    done

    kill "$pidlist" 2> /dev/null
}

# run shutdown
trap shutdown SIGINT
wait -n

# return received result
exit $latest_exit