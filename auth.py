# Modified based on https://github.com/Azure-Samples/ms-identity-python-webapi-azurefunctions/blob/master/Function/secureFlaskApp/__init__.py

from jose import jwt
import json


class AuthError(Exception):
    def __init__(self, error, status_code):
        self.error = error
        self.status_code = status_code


def handle_auth_error(ex):
    return json.dumps(ex.__dict__)


def get_token_auth_header(request):
    auth = request.headers.get("Authorization", None)
    if not auth:
        raise AuthError({"code": "authorization_header_missing",
                         "description":
                         "Authorization header is expected"}, 401)
    parts = auth.split()
    if parts[0].lower() != "bearer":
        raise AuthError({"code": "invalid_header",
                         "description":
                         "Authorization header must start with"
                         " Bearer"}, 401)
    elif len(parts) == 1:
        raise AuthError({"code": "invalid_header",
                         "description": "Token not found"}, 401)
    elif len(parts) > 2:
        raise AuthError({"code": "invalid_header",
                         "description":
                         "Authorization header must be"
                         " Bearer token"}, 401)

    token = parts[1]

    return token


def verify_roles(request, required_role):
    verified = False
    token = get_token_auth_header(request)
    unverified_claims = jwt.get_unverified_claims(token)

    if unverified_claims.get("roles"):
        token_roles = unverified_claims["roles"]
        for token_role in token_roles:
            if token_role == required_role:
                verified = True

    if not verified:
        raise AuthError({"code": "unauthorized_roles",
                         "description":
                         "Roles are not authorized for this operation"}, 401)
