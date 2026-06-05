<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);
$id_playthrough = isset($input['id_playthrough']) ? (int)$input['id_playthrough'] : null;
$device_type = isset($input['device_type']) ? trim($input['device_type']) : null;

if (!$id_playthrough) {
    response_error("El ID de la partida (playthrough) es obligatorio.");
}

try {
    $stmt = $pdo->prepare("CALL sp_insert_game_session(:id_playthrough, :device_type)");
    $stmt->bindParam(':id_playthrough', $id_playthrough, PDO::PARAM_INT);
    $stmt->bindParam(':device_type', $device_type);
    $stmt->execute();

    $result = $stmt->fetch();
    $stmt->closeCursor();

    response_success("✅ Sesión creada correctamente", [
        "id_session" => $result['new_id']
    ]);

} catch (PDOException $e) {
    response_error("❌ Error al crear la sesión", $e->getMessage());
}
