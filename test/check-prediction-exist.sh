#!/bin/bash
PREDICTION_RESULT="src/dataset/movies.pred.tsv"

if [[ ! -f $PREDICTION_RESULT ]] ; then
    echo "File ${PREDICTION_RESULT} is not there, aborting."
    exit 1
fi