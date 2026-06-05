<?php
require_once 'config/database.php';

header('Content-Type: application/json');

echo json_encode([
    "status" => "✅ Conexión establecida correctamente",
    "host" => $_ENV['DB_HOST'],
    "database" => $_ENV['DB_NAME'],
    "user" => $_ENV['DB_USER']
]);
