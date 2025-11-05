import logging
import os

def setup_logger():
    log_level = os.getenv("LOG_LEVEL", "INFO").upper()
    logging.basicConfig(
        format="%(asctime)s | %(levelname)s | %(message)s",
        level=log_level
    )
    return logging.getLogger("sos_analytics")

logger = setup_logger()
