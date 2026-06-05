<?php

require_once __DIR__ . '/env.php';

loadEnv(__DIR__ . '/../.env');

$host = envValue('DB_HOST', envValue('MYSQLHOST'));
$port = envValue('DB_PORT', envValue('MYSQLPORT', '3306'));
$dbname = envValue('DB_NAME', envValue('MYSQLDATABASE'));
$user = envValue('DB_USER', envValue('MYSQLUSER'));

// Soporta DB_PASS y DB_PASSWORD
$password = envValue('DB_PASS', envValue('DB_PASSWORD', envValue('MYSQLPASSWORD')));

if (!$host || !$port || !$dbname || !$user || !$password) {
    http_response_code(500);
    echo json_encode([
        'error' => true,
        'message' => 'Missing database environment variables',
        'has_host' => (bool)$host,
        'has_port' => (bool)$port,
        'has_dbname' => (bool)$dbname,
        'has_user' => (bool)$user,
        'has_password' => (bool)$password
    ]);
    exit;
}

try {
    $pdo = new PDO(
        "mysql:host=$host;port=$port;dbname=$dbname;charset=utf8mb4",
        $user,
        $password,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        ]
    );
} catch (PDOException $e) {
    http_response_code(500);
    echo json_encode([
        'error' => true,
        'message' => '❌ Error de conexión a la base de datos',
        'debug' => [
            'pdo_message' => $e->getMessage(),
            'host_set' => !empty($host),
            'port_set' => !empty($port),
            'dbname_set' => !empty($dbname),
            'user_set' => !empty($user),
            'pass_set' => !empty($password),
            'host' => $host,
            'port' => $port,
            'dbname' => $dbname,
            'user' => $user
        ]
    ]);
    exit;
}