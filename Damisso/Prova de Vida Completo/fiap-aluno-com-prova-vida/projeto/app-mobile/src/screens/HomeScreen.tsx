import { useState } from 'react';
import {
  ActivityIndicator,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';
import {
  Pensionista,
  StatusProvaVida,
  buscarPensionistaPorNuit,
  consultarStatusProvaVida,
} from '../services/api';

type Props = {
  onIniciar: (pensionista: Pensionista, status: StatusProvaVida) => void;
};

export function HomeScreen({ onIniciar }: Props) {
  const [nuit, setNuit] = useState('300400504');
  const [carregando, setCarregando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [status, setStatus] = useState<StatusProvaVida | null>(null);

  async function continuar() {
    setCarregando(true);
    setErro(null);
    setStatus(null);

    try {
      const nuitLimpo = nuit.trim();
      const pensionista = await buscarPensionistaPorNuit(nuitLimpo);
      if (!pensionista) {
        setErro('NUIT não encontrado ou não é pensionista.');
        return;
      }

      const statusProva = await consultarStatusProvaVida(nuitLimpo);
      setStatus(statusProva);
      onIniciar(pensionista, statusProva);
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro de conexão com a API.');
    } finally {
      setCarregando(false);
    }
  }

  return (
    <View style={styles.container}>
      <Text style={styles.titulo}>INSS Moçambique</Text>
      <Text style={styles.subtitulo}>Prova de Vida — NUIT + reconhecimento facial</Text>

      <Text style={styles.label}>NUIT do pensionista (documento — como CPF no Brasil)</Text>
      <TextInput
        style={styles.input}
        value={nuit}
        onChangeText={setNuit}
        keyboardType="number-pad"
        maxLength={9}
        placeholder="Ex.: 300400504"
      />

      <Text style={styles.dica}>
        Identidade por NUIT + template 128D no banco (não salva imagem).{'\n'}
        Demo: 300400504 · match endurecido ≥ 72% · mão no rosto tende a falhar
      </Text>

      {status ? (
        <View style={[styles.statusBox, status.provaValida ? styles.statusOk : styles.statusWarn]}>
          <Text style={styles.statusTexto}>
            {status.provaValida
              ? `Prova válida — ${status.diasRestantes} dias · Continuar abre a câmera`
              : status.proximaAcao === 'Cadastrar'
                ? 'Sem cadastro facial — 1ª vez'
                : 'Prova expirada — renovar com rosto cadastrado'}
          </Text>
        </View>
      ) : null}

      {erro ? <Text style={styles.erro}>{erro}</Text> : null}

      <Pressable style={styles.botao} onPress={continuar} disabled={carregando}>
        {carregando ? (
          <ActivityIndicator color="#fff" />
        ) : (
          <Text style={styles.botaoTexto}>Continuar</Text>
        )}
      </Pressable>

      <Text style={styles.nota}>
        Celular físico: PC e telefone na mesma Wi‑Fi · APIs em app.json{'\n'}
        1ª execução baixa modelos face-api (~6 MB).
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, padding: 24, justifyContent: 'center', backgroundColor: '#f5f7fb' },
  titulo: { fontSize: 26, fontWeight: '800', color: '#0d47a1' },
  subtitulo: { marginTop: 4, fontSize: 16, color: '#333' },
  label: { marginTop: 32, fontWeight: '600' },
  input: {
    marginTop: 8,
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 12,
    backgroundColor: '#fff',
    fontSize: 18,
  },
  dica: { marginTop: 8, color: '#666', fontSize: 12, lineHeight: 18 },
  statusBox: { marginTop: 12, padding: 10, borderRadius: 8 },
  statusOk: { backgroundColor: '#e8f5e9' },
  statusWarn: { backgroundColor: '#fff8e1' },
  statusTexto: { fontSize: 13, color: '#333' },
  botao: {
    marginTop: 20,
    backgroundColor: '#0d47a1',
    padding: 14,
    borderRadius: 8,
    alignItems: 'center',
  },
  botaoTexto: { color: '#fff', fontWeight: '700', fontSize: 16 },
  erro: { marginTop: 12, color: '#b00020' },
  nota: { marginTop: 24, color: '#777', fontSize: 12, lineHeight: 18 },
});
