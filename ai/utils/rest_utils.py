import requests
from typing import Dict, Any, Optional
import json


def post(
    url: str,
    data: Dict[str, Any],
    access_token: Optional[str] = None,
    headers: Optional[Dict[str, str]] = None,
    verify_ssl: bool = False,
) -> tuple[bool, Optional[str]]:
    """
    Make a POST request to a REST API endpoint.

    Args:
        url: The URL of the REST API endpoint.
        data: The data to be sent in the request body.
        headers: Optional headers to be included in the request.
        verify_ssl: Whether to verify SSL certificates. Set to False to disable SSL verification.

    Returns:
        The JSON response from the API as a dictionary.

    Raises:
        HTTPException: If the request fails or returns a non-200 status code.
    """
    try:
        default_headers = {
            "Content-Type": "application/json",
            "Accept": "application/json",
        }

        if access_token:
            default_headers["Authorization"] = f"Bearer {access_token}"

        if headers:
            default_headers.update(headers)

        response = requests.post(
            url=url, data=json.dumps(data), headers=default_headers, verify=verify_ssl
        )

        response.raise_for_status()  # Raise an exception for 4XX/5XX responses

        return True, response.json()
    except requests.exceptions.RequestException as e:
        # Handle SSL verification errors with a more specific message
        if isinstance(e, requests.exceptions.SSLError):
            return (
                False,
                f"SSL certificate verification failed. You may need to set verify_ssl=False if using a self-signed certificate. {str(e)}",
            )
        return False, f"REST API request failed: {str(e)}"
    except json.JSONDecodeError:
        return False, "Failed to parse JSON response"
    except Exception as e:
        return False, str(e)
