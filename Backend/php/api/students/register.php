<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

// Validar método
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

// Obtener datos
$input = json_decode(file_get_contents("php://input"), true);
$ci = isset($input['ci']) ? trim($input['ci']) : null;

// Validación básica
if (!$ci) {
    response_error("CI es obligatorio.");
}

try {
    $stmt = $pdo->prepare("CALL sp_insert_student(:ci)");
    $stmt->bindParam(':ci', $ci, PDO::PARAM_STR);
    $stmt->execute();

    response_success("✅ Estudiante registrado correctamente", ["ci" => $ci]);

} catch (PDOException $e) {
    response_error("❌ Error al registrar estudiante", $e->getMessage());
}
