##############################
# LIGHT IN THE ABYSS
##############################
use_bpm 40
set_volume! 1.5

##############################
# CONFIGURACIÓN DE TIEMPOS
##############################

tiempo_thanatos = 20
tiempo_silencio = 7
tiempo_total = 165  # 2:45

# Reloj en segundos reales
vt = 0
live_loop :reloj do
  set :tiempo, vt
  vt += 1
  sleep 1
end

define :t do
  get(:tiempo)
end

define :en_preludio? do
  t < tiempo_thanatos
end

define :musica_activa? do
  t >= (tiempo_thanatos + tiempo_silencio) and t < tiempo_total - 5
end

define :cierre? do
  t >= tiempo_total - 5
end

define :transicion_amp do |desde, hasta, valor_inicio, valor_fin|
  escala = valor_fin - valor_inicio
  factor = [[(t - desde).to_f / (hasta - desde), 0].max, 1].min
  return valor_inicio + factor * escala
end

##############################
# CAJA MUSICAL
##############################

define :caja_melodica do
  use_synth :pretty_bell
  with_fx :reverb, mix: 0.5, room: 0.8 do
    play_pattern_timed [:e4, :g4, :b4, :e5], [1.5, 1.5, 1, 2], release: 2, amp: 0.25
  end
end

##############################
# THANATOS: PRELUDIO
##############################

live_loop :thanatos do
  if en_preludio?
    use_synth :dark_ambience
    play choose([:e1, :d1, :c2]), attack: 2, release: 5, amp: 0.3
    sleep 5
  else
    sleep 1
  end
end

live_loop :sombra_respira do
  if t < tiempo_total - 5
    sample :ambi_drone, rate: 0.2, amp: 0.12
    sleep 8
  else
    sleep 1
  end
end

live_loop :eco_abismo do
  if t < tiempo_total - 5
    sample :ambi_soft_buzz, rate: 0.3, amp: 0.05
    sleep 12
  else
    sleep 1
  end
end

##############################
# EROS + MÚSICA ACTIVA
##############################

live_loop :eros_latido do
  if musica_activa?
    use_synth :pulse
    with_fx :lpf, cutoff: 70 do
      play :e3, amp: 0.08, attack: 0.01, sustain: 0.1, release: 0.2
      sleep 0.4
      play :e3, amp: 0.06, attack: 0.01, sustain: 0.05, release: 0.2
    end
    sleep 2.2
  else
    sleep 1
  end
end

with_fx :reverb, mix: 0.8, room: 0.9 do
  with_fx :compressor do
    
    live_loop :eco_antiguo do
      if musica_activa?
        sample :ambi_glass_hum, rate: 0.25, amp: 0.3
        sleep 16
      else
        sleep 1
      end
    end
    
    live_loop :tema_caja do
      if musica_activa?
        caja_melodica
        sleep 4
      else
        sleep 1
      end
    end
    
    live_loop :latido_de_hierro do
      if musica_activa?
        use_synth :mod_beep
        play choose([:e2, :g2]), release: 1.5, amp: 0.1
        sleep [3, 4].choose
      else
        sleep 1
      end
    end
    
    live_loop :coro_fantasma do
      if musica_activa?
        use_synth :hollow
        notas = chord(:e3, :minor).shuffle
        with_fx :reverb, mix: 0.9, room: 1 do
          2.times do
            play notas.tick + [0, 0.02, -0.03].choose, attack: 2, sustain: 3, release: 4, amp: 0.15
            sleep [2.5, 3.5].choose
          end
        end
        sleep 4
      else
        sleep 1
      end
    end
    
    live_loop :fragmentos_perdidos do
      if musica_activa?
        use_synth :pretty_bell
        with_fx :echo, mix: 0.3, phase: 0.5 do
          play choose([:g4, :b4, :c5, :d5]), release: 1.5, amp: 0.12
          sleep [1.5, 2, 3].choose
        end
      else
        sleep 1
      end
    end
    
    live_loop :sombra_cercana do
      if musica_activa?
        use_synth :fm
        with_fx :distortion, mix: 0.4 do
          play choose([:e1, :d1]), attack: 0.3, release: 2.5, amp: 0.22
          sleep [7, 8].choose
        end
      else
        sleep 1
      end
    end
    
  end
end

##############################
# CIERRE: LA PREGUNTA
##############################

live_loop :pregunta_final do
  if cierre?
    stop
  end
  
  if t == tiempo_total - 5
    in_thread do
      sleep 3.5
      
      use_synth :hollow
      with_fx :reverb, mix: 0.9, room: 1 do
        play chord(:e4, :sus2), release: 6, amp: 0.3
      end
      
      sleep 1
      
      use_synth :pretty_bell
      play :b4, release: 6, amp: 0.2
    end
    stop
  end
  sleep 1
end
