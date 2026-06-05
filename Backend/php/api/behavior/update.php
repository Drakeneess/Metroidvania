<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);
$id_summary = isset($input['id_summary']) ? (int)$input['id_summary'] : null;
$inactivity = isset($input['inactivity_periods']) ? (int)$input['inactivity_periods'] : 0;
$actions = isset($input['total_actions']) ? (int)$input['total_actions'] : 0;
$social = isset($input['social_interactions']) ? (int)$input['social_interactions'] : 0;

if (!$id_summary) {
    response_error("El ID del summary es obligatorio.");
}

try {
    $stmt = $pdo->prepare("CALL sp_update_insert_behavior_summary(:id_summary, :inactivity, :actions, :social)");
    $stmt->bindParam(':id_summary', $id_summary, PDO::PARAM_INT);
    $stmt->bindParam(':inactivity', $inactivity, PDO::PARAM_INT);
    $stmt->bindParam(':actions', $actions, PDO::PARAM_INT);
    $stmt->bindParam(':social', $social, PDO::PARAM_INT);
    $stmt->execute();
    $stmt->closeCursor();

    response_success("✅ Behavior actualizado correctamente", [
        "id_summary" => $id_summary,
        "inactivity_periods" => $inactivity,
        "total_actions" => $actions,
        "social_interactions" => $social
    ]);

} catch (PDOException $e) {
    response_error("❌ Error al actualizar behavior", $e->getMessage());
}
