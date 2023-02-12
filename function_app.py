import azure.functions as func
import logging
from data_handler import get_recommendation

app = func.FunctionApp()

# Learn more at aka.ms/pythonprogrammingmodel

# Get started by running the following code to create a function using a HTTP trigger.

@app.function_name(name="MoviesRecommender")
@app.route(route="movies", auth_level=func.AuthLevel.ANONYMOUS)
def get_movies_recommendation(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    movies_result = get_recommendation()
    print(movies_result)
    
    return func.HttpResponse(
        movies_result,
        status_code=200,
        mimetype='application/json'
    )
        