<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    response_error("Método no permitido", "Usa GET.");
}

$id_student = isset($_GET['id_student']) ? (int)$_GET['id_student'] : null;

if (!$id_student) {
    response_error("Falta el parámetro id_student.");
}

try {
    $stmt = $pdo->prepare("SELECT fn_get_latest_playthrough_id_by_student(:id_student) AS id_playthrough");
    $stmt->bindParam(':id_student', $id_student, PDO::PARAM_INT);
    $stmt->execute();

    $result = $stmt->fetch();
    $stmt->closeCursor();

    if ($result && $result['id_playthrough']) {
        response_success("✅ Última partida encontrada", [
            "id_playthrough" => $result['id_playthrough']
        ]);
    } else {
        response_success("El estudiante no tiene partidas registradas.", [
            "id_playthrough" => null
        ]);
    }

} catch (PDOException $e) {
    response_error("❌ Error al obtener la última partida", $e->getMessage());
}
