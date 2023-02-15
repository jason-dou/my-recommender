import azure.functions as func
import logging

from data_handler import get_recommendation
from auth import AuthError, verify_roles, handle_auth_error

app = func.FunctionApp()


@app.function_name(name="MoviesRecommender")
@app.route(route="movies", auth_level=func.AuthLevel.ANONYMOUS)
def get_movies_recommendation(req: func.HttpRequest) -> func.HttpResponse:
    try:
        logging.info('Getting Movies Recommendation Request.')
        # verify_roles(req, "Recommendation.Read")
        movies_result = get_recommendation()

        return func.HttpResponse(
            movies_result,
            status_code=200,
            mimetype='application/json'
        )
    except AuthError as ae:
        logging.error(ae)
        error_response = handle_auth_error(ae)

        return func.HttpResponse(
            error_response,
            status_code=401,
            mimetype='application/json'
        )
    except Exception as e:
        logging.error(e)
        return func.HttpResponse(
            "Internal Error",
            status_code=500,
        )
