<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);
$id_playthrough = isset($input['id_playthrough']) ? (int)$input['id_playthrough'] : null;
$id_response = isset($input['id_response']) ? (int)$input['id_response'] : null;

if (!$id_playthrough || !$id_response) {
    response_error("Parámetros 'id_playthrough' y 'id_response' son obligatorios.");
}

try {
    // Ejecutar el procedimiento almacenado
    $stmt = $pdo->prepare("CALL sp_insert_bdi_test_result(:id_playthrough, :id_response)");
    $stmt->bindParam(':id_playthrough', $id_playthrough, PDO::PARAM_INT);
    $stmt->bindParam(':id_response', $id_response, PDO::PARAM_INT);
    $stmt->execute();

    // Si el SP devuelve algún resultado (ej: id_test insertado)
    $result = $stmt->fetch();
    $stmt->closeCursor();

    response_success("✅ Respuesta de BDI registrada", [
        "id_playthrough" => $id_playthrough,
        "id_response" => $id_response,
        "id_test" => $result['id_test'] ?? null
    ]);

} catch (PDOException $e) {
    response_error("❌ Error al guardar la respuesta BDI", $e->getMessage());
}
