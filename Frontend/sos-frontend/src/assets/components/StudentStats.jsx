import { safeNum } from "./utils";
import { Grid, Card, Label, Big, Medium, Small, Progress } from "./UIHelpers";

export default function StudentStats({ stats = {} }) {
  return (
    <Grid cols={3}>
      <Card>
        <Label>Puntaje total</Label>
        <Big>{safeNum(stats.totalScore)}</Big>
      </Card>
      <Card>
        <Label>Avance</Label>
        <Progress value={safeNum(stats.completion)} />
        <Small>
          {safeNum(stats.answeredCount)}/{safeNum(stats.totalItems)} (
          {safeNum(stats.completion)}%)
        </Small>
      </Card>
      <Card>
        <Label>Última respuesta</Label>
        <Medium>
          {stats.lastAnswerAt ? new Date(stats.lastAnswerAt).toLocaleString() : "—"}
        </Medium>
      </Card>
    </Grid>
  );
}
