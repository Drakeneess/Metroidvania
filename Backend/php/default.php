<?php

header('Content-Type: application/json; charset=utf-8');

$method = $_SERVER['REQUEST_METHOD'];
$path = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);

// Health check
if ($method === 'GET' && $path === '/api/health') {
    echo json_encode([
        'status' => 'ok',
        'message' => 'PHP API running',
        'path' => $path
    ]);
    exit;
}

// Permitir rutas tipo /api/students/register.php
if (str_starts_with($path, '/api/')) {
    $relativePath = substr($path, strlen('/api/'));

    // Seguridad básica: evitar traversal tipo ../../
    if (str_contains($relativePath, '..')) {
        http_response_code(400);
        echo json_encode([
            'status' => 'error',
            'message' => 'Invalid path'
        ]);
        exit;
    }

    $targetFile = __DIR__ . '/api/' . $relativePath;

    if (is_file($targetFile)) {
        require_once $targetFile;
        exit;
    }
}

http_response_code(404);
echo json_encode([
    'status' => 'error',
    'message' => 'Route not found',
    'path' => $path
]);