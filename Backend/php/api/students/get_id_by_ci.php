<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    response_error("Método no permitido", "Usa GET.");
}

$ci = isset($_GET['ci']) ? trim($_GET['ci']) : null;

if (!$ci) {
    response_error("Falta el parámetro ci.");
}

try {
    $stmt = $pdo->prepare("SELECT fn_get_student_id_by_ci(:ci) AS id_student");
    $stmt->bindParam(':ci', $ci);
    $stmt->execute();

    $result = $stmt->fetch();
    $stmt->closeCursor();

    if ($result && $result['id_student']) {
        response_success("✅ ID del estudiante encontrado", [
            "id_student" => $result['id_student']
        ]);
    } else {
        response_success("No se encontró estudiante con ese CI.", [
            "id_student" => null
        ]);
    }

} catch (PDOException $e) {
    response_error("❌ Error al obtener el ID del estudiante", $e->getMessage());
}
