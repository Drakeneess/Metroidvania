<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);
$id_student = isset($input['id_student']) ? (int)$input['id_student'] : null;
$version = isset($input['version']) ? trim($input['version']) : null;

if (!$id_student || !$version) {
    response_error("Parámetros 'id_student' y 'version' son obligatorios.");
}

try {
    // Buscar si ya existe una partida con ese id_student y versión
    $stmt = $pdo->prepare("SELECT fn_get_playthrough_id_by_student_and_version(:id_student, :version) AS id_playthrough");
    $stmt->bindParam(':id_student', $id_student, PDO::PARAM_INT);
    $stmt->bindParam(':version', $version);
    $stmt->execute();

    $result = $stmt->fetch();
    $stmt->closeCursor();

    if ($result && $result['id_playthrough']) {
        response_success("⚠️ Ya existe una partida con esta versión.", [
            "id_playthrough" => $result['id_playthrough'],
            "existing" => true
        ]);
    }

    // Crear la partida porque no existe aún
    $stmt = $pdo->prepare("CALL sp_insert_playthrough(:id_student, :version)");
    $stmt->bindParam(':id_student', $id_student, PDO::PARAM_INT);
    $stmt->bindParam(':version', $version);
    $stmt->execute();

    $created = $stmt->fetch();
    $stmt->closeCursor();

    response_success("✅ Nueva partida creada", [
        "id_playthrough" => $created['new_id'],
        "existing" => false
    ]);

} catch (PDOException $e) {
    response_error("❌ Error al verificar o crear la partida", $e->getMessage());
}
