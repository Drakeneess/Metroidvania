<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

// Recibir JSON
$input = json_decode(file_get_contents("php://input"), true);
$id_session = $input['id_session'] ?? null;
$log_data   = $input['log_data'] ?? null;

if (!$id_session || !$log_data) {
    response_error("Faltan parámetros", "Debes enviar id_session y log_data");
}

// 🔹 Subir a Python
$python_api = "https://uploadbehaviour-production.up.railway.app/upload";

$ch = curl_init($python_api);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_POST, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, ["Content-Type: application/json"]);
curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode([
    "id_session" => $id_session,
    "log_data"   => $log_data
]));

$response = curl_exec($ch);
$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);

if ($httpcode !== 200) {
    response_error("❌ Error al subir log", $response);
}

// 🔹 Guardar referencia en SQL (ejecutar SP)
try {
    $stmt = $pdo->prepare("CALL sp_close_game_session(:id_session, :nosql_log_id)");
    $stmt->bindParam(':id_session', $id_session, PDO::PARAM_INT);

    // `nosql_log_id` viene del JSON que responde Python
    $nosql_response = json_decode($response, true);
    $nosql_log_id = $nosql_response['id'] ?? null;

    if (!$nosql_log_id) {
        response_error("❌ Python no devolvió un nosql_log_id válido", $nosql_response);
    }

    $stmt->bindParam(':nosql_log_id', $nosql_log_id);
    $stmt->execute();

    response_success("✅ Sesión cerrada y log registrado", [
        "id_session"    => $id_session,
        "nosql_log_id"  => $nosql_log_id
    ]);
} catch (PDOException $e) {
    response_error("❌ Error al cerrar sesión en SQL", $e->getMessage());
}
