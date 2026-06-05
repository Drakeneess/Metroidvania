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
    $stmt = $pdo->prepare("CALL sp_get_playthroughs_by_student(:id_student)");
    $stmt->bindParam(':id_student', $id_student, PDO::PARAM_INT);
    $stmt->execute();

    $rows = $stmt->fetchAll();
    $stmt->closeCursor();

    if (!$rows) {
        response_success("No se encontraron partidas registradas para este estudiante.", []);
    } else {
        response_success("✅ Partidas encontradas", $rows);
    }

} catch (PDOException $e) {
    response_error("❌ Error al obtener partidas", $e->getMessage());
}
