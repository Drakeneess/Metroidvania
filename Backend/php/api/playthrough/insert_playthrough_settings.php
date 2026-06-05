<?php
require_once __DIR__ . '/../../config/database.php';
require_once __DIR__ . '/../../helpers/response.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    response_error("Método no permitido", "Usa POST.");
}

$input = json_decode(file_get_contents("php://input"), true);

$id_playthrough = isset($input['id_playthrough']) ? (int)$input['id_playthrough'] : null;
$Music       = $input['Music']       ?? null;
$Fx          = $input['Fx']          ?? null;
$Lang        = $input['Lang']        ?? null;
$Rumbling    = $input['Rumbling']    ?? null;
$Resolution  = $input['Resolution']  ?? null;
$Bright      = $input['Bright']      ?? null;

if (!$id_playthrough) {
    response_error("El ID de la partida (id_playthrough) es obligatorio.");
}

try {
    $stmt = $pdo->prepare("CALL sp_insert_playthrough_settings(:id_playthrough, :Music, :Fx, :Lang, :Rumbling, :Resolution, :Bright)");

    $stmt->bindParam(':id_playthrough', $id_playthrough, PDO::PARAM_INT);

    // Usa PARAM_NULL si llega null
    $stmt->bindValue(':Music',      $Music,      is_null($Music) ? PDO::PARAM_NULL : PDO::PARAM_INT);
    $stmt->bindValue(':Fx',         $Fx,         is_null($Fx) ? PDO::PARAM_NULL : PDO::PARAM_INT);
    $stmt->bindValue(':Lang',       $Lang,       is_null($Lang) ? PDO::PARAM_NULL : PDO::PARAM_INT);
    $stmt->bindValue(':Rumbling',   $Rumbling,   is_null($Rumbling) ? PDO::PARAM_NULL : PDO::PARAM_INT);
    $stmt->bindValue(':Resolution', $Resolution, is_null($Resolution) ? PDO::PARAM_NULL : PDO::PARAM_INT);
    $stmt->bindValue(':Bright',     $Bright,     is_null($Bright) ? PDO::PARAM_NULL : PDO::PARAM_INT);

    $stmt->execute();
    $stmt->closeCursor();

    response_success("✅ Configuración registrada correctamente.");

} catch (PDOException $e) {
    response_error("❌ Error al registrar configuración", $e->getMessage());
}
