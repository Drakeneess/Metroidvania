############################
# UNPROBABLE HOME
############################

use_bpm 45

with_fx :reverb, mix: 0.6, room: 0.85 do
  with_fx :compressor do
    
    # Acordes suspendidos: flotantes, sin resolución inmediata
    live_loop :asamblea_acorde do
      use_synth :hollow
      chord_prog = [
        chord(:e3, :minor7),
        chord(:a3, :sus2),
        [:g3, :b3, :f4],   # En vez de :g3, :major7
        [:f3, :ab3, :c4, :eb4, :g4]
      ]
      
      play chord_prog.tick, attack: 2, sustain: 4, release: 3, amp: 0.4
      sleep 8
    end
    
    # Pensamientos flotantes – Melodía aleatoria con eco
    live_loop :asamblea_melodia, sync: :asamblea_acorde do
      use_synth :beep
      notes = [:g3, :a3, :c4, :d4]
      with_fx :echo, mix: 0.5, phase: 0.75 do
        notes = [:e5, :g4, :b4, :d5, :a4]
        3.times do
          play notes.choose, release: 1.5, amp: 0.18
          sleep [1.5, 2].choose
        end
      end
    end
    
    # Fondo atmosférico suave – bruma
    live_loop :asamblea_niebla, sync: :asamblea_acorde do
      sample :ambi_soft_buzz, rate: 0.3, amp: 0.25
      sleep 16
    end
    
    # Pulso estable – el tiempo dentro de la Asamblea
    live_loop :asamblea_pulso, sync: :asamblea_acorde do
      use_synth :piano
      play :e2, attack: 0.1, release: 1.5, amp: 0.12
      sleep 6
    end
    
    # Coro tenue – como un juicio sin palabras
    live_loop :asamblea_coro, sync: :asamblea_acorde do
      use_synth :dark_ambience
      with_fx :reverb, mix: 0.8, room: 1 do
        play [:e3, :f3, :g3], attack: 2, release: 6, amp: 0.12
      end
      sleep 16
    end
    
    
    # Ruido invertido – distorsión del tiempo
    live_loop :asamblea_recuerdo, sync: :asamblea_acorde do
      with_fx :slicer, phase: 8, invert_wave: 1, mix: 0.2 do
        sample :ambi_choir, rate: -0.25, amp: 0.1
      end
      sleep 24
    end
    
  end
end
