import { Pressable, StyleSheet, Text, View } from 'react-native';

type Props = {
  registroId: string;
  tipoOperacao: string;
  similaridadeFacial: number | null;
  onNovaProva: () => void;
};

export function SuccessScreen({ registroId, tipoOperacao, similaridadeFacial, onNovaProva }: Props) {
  const tipo = tipoOperacao.toLowerCase();
  const consulta = tipo.includes('consulta');
  const renovacao = tipo.includes('renov');

  const titulo = consulta
    ? 'Identidade confirmada'
    : renovacao
      ? 'Prova renovada'
      : 'Cadastro facial concluído';

  const texto = consulta
    ? 'Rosto autenticado no cadastro. Benefício continua liberado — validade não foi alterada.'
    : renovacao
      ? 'Rosto validado contra o cadastro existente. Benefício renovado por mais 12 meses.'
      : 'Template facial 128D salvo. Nas próximas vezes, a câmera autentica sem criar outro cadastro.';

  return (
    <View style={styles.container}>
      <Text style={styles.titulo}>{titulo}</Text>
      <Text style={styles.texto}>{texto}</Text>
      {similaridadeFacial != null ? (
        <Text style={styles.similaridade}>
          Similaridade facial: {similaridadeFacial.toFixed(0)}% (mín. 72%)
        </Text>
      ) : null}
      <Text style={styles.id}>Registro: {registroId}</Text>
      <Pressable style={styles.botao} onPress={onNovaProva}>
        <Text style={styles.botaoTexto}>Nova consulta</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, justifyContent: 'center', padding: 24, backgroundColor: '#f5f7fb' },
  titulo: { fontSize: 24, fontWeight: '800', color: '#1b5e20' },
  texto: { marginTop: 12, fontSize: 16, color: '#333', lineHeight: 22 },
  similaridade: { marginTop: 12, fontSize: 15, fontWeight: '600', color: '#0d47a1' },
  id: { marginTop: 8, fontSize: 12, color: '#666' },
  botao: {
    marginTop: 24,
    backgroundColor: '#0d47a1',
    padding: 14,
    borderRadius: 8,
    alignItems: 'center',
  },
  botaoTexto: { color: '#fff', fontWeight: '700' },
});
