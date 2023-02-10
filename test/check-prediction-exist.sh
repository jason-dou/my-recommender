#!/bin/bash
PREDICTION_RESULT="src/dataset/movies.pred.1tsv"

if [[ ! -f $PREDICTION_RESULT ]] ; then
    echo "File ${PREDICTION_RESULT} is not there, aborting."
    exit 1
fi