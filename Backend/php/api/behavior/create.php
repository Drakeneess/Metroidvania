<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);
$id_playthrough = isset($input['id_playthrough']) ? (int)$input['id_playthrough'] : null;

if (!$id_playthrough) {
    response_error("El ID del playthrough es obligatorio.");
}

try {
    $stmt = $pdo->prepare("CALL sp_insert_behavior_summary(:id_playthrough, @new_id_summary)");
    $stmt->bindParam(':id_playthrough', $id_playthrough, PDO::PARAM_INT);
    $stmt->execute();
    $stmt->closeCursor();

    // Obtener el valor OUT
    $result = $pdo->query("SELECT @new_id_summary AS id_summary")->fetch();

    response_success("✅ Behavior creado/obtenido correctamente", [
        "id_summary" => $result['id_summary']
    ]);

} catch (PDOException $e) {
    response_error("❌ Error al crear/obtener behavior", $e->getMessage());
}
