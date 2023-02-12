import pandas as pd

def get_recommendation():
    pred = pd.read_table('src/dataset/movies.pred.tsv')
    min_score = 3.0
    cols_display = ['numVotes', 'titleType', 'primaryTitle', 'Score', 'startYear', 'genres']
    high_score_movies = pred.loc[(pred['Score'] > min_score)]
    movies_result = high_score_movies[cols_display]
    
    return movies_result.to_json(orient='records')
