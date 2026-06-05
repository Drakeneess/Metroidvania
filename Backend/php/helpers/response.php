<?php

function response_success($message, $data = null) {
    echo json_encode([
        "success" => true,
        "message" => $message,
        "data" => $data
    ]);
    exit;
}

function response_error($message, $error = null) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "message" => $message,
        "error" => $error
    ]);
    exit;
}
