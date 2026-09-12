import './src/polyfills/rnFaceEnv';
import { useState } from 'react';
import { StatusBar } from 'expo-status-bar';
import { SafeAreaView, StyleSheet } from 'react-native';
import { HomeScreen } from './src/screens/HomeScreen';
import { LivenessScreen } from './src/screens/LivenessScreen';
import { SuccessScreen } from './src/screens/SuccessScreen';
import { Pensionista, StatusProvaVida } from './src/services/api';

type Tela = 'home' | 'liveness' | 'success';

export default function App() {
  const [tela, setTela] = useState<Tela>('home');
  const [pensionista, setPensionista] = useState<Pensionista | null>(null);
  const [status, setStatus] = useState<StatusProvaVida | null>(null);
  const [registroId, setRegistroId] = useState('');
  const [tipoOperacao, setTipoOperacao] = useState('');
  const [similaridadeFacial, setSimilaridadeFacial] = useState<number | null>(null);

  function reiniciar() {
    setPensionista(null);
    setStatus(null);
    setRegistroId('');
    setTipoOperacao('');
    setSimilaridadeFacial(null);
    setTela('home');
  }

  return (
    <SafeAreaView style={styles.root}>
      <StatusBar style="dark" />
      {tela === 'home' && (
        <HomeScreen
          onIniciar={(p, s) => {
            setPensionista(p);
            setStatus(s);
            setTela('liveness');
          }}
        />
      )}
      {tela === 'liveness' && pensionista && status && (
        <LivenessScreen
          pensionista={pensionista}
          status={status}
          onSuccess={({ id, similaridadeFacial: sim, tipoOperacao: tipo }) => {
            setRegistroId(id);
            setSimilaridadeFacial(sim);
            setTipoOperacao(tipo);
            setTela('success');
          }}
          onCancel={reiniciar}
        />
      )}
      {tela === 'success' && (
        <SuccessScreen
          registroId={registroId}
          tipoOperacao={tipoOperacao}
          similaridadeFacial={similaridadeFacial}
          onNovaProva={reiniciar}
        />
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  root: { flex: 1, backgroundColor: '#f5f7fb' },
});
