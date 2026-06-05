<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);
$id_student = isset($input['id_student']) ? (int)$input['id_student'] : null;
$version = isset($input['version']) ? trim($input['version']) : null;

// Si no viene nada, se queda en null
if (!$id_student) {
    response_error("El ID del estudiante es obligatorio.");
}

try {
    $stmt = $pdo->prepare("CALL sp_insert_playthrough(:id_student, :version)");
    $stmt->bindParam(':id_student', $id_student, PDO::PARAM_INT);
    $stmt->bindParam(':version', $version);
    $stmt->execute();

    $result = $stmt->fetch();
    $stmt->closeCursor();

    response_success("✅ Partida creada correctamente", [
        "id_playthrough" => $result['new_id']
    ]);

} catch (PDOException $e) {
    response_error("❌ Error al crear la partida", $e->getMessage());
}
