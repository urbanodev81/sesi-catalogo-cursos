import { mount } from '@vue/test-utils'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { defineComponent, h, nextTick, ref, type Ref } from 'vue'
import { DEMORA_MS, useDemora } from '../src/composables/useDemora'

function montar(esperando: Ref<boolean>) {
  let demorou!: Ref<boolean>
  mount(
    defineComponent({
      setup() {
        demorou = useDemora(esperando)
        return () => h('div')
      },
    }),
  )
  return () => demorou.value
}

beforeEach(() => vi.useFakeTimers())
afterEach(() => vi.useRealTimers())

describe('useDemora (aviso de servidor acordando)', () => {
  it('só avisa depois de 5 s seguidos carregando', () => {
    const demorou = montar(ref(true))

    vi.advanceTimersByTime(DEMORA_MS - 1)
    expect(demorou()).toBe(false)

    vi.advanceTimersByTime(1)
    expect(demorou()).toBe(true)
  })

  it('carga rápida nunca mostra o aviso', async () => {
    const esperando = ref(true)
    const demorou = montar(esperando)

    vi.advanceTimersByTime(1_000)
    esperando.value = false
    await nextTick()
    vi.advanceTimersByTime(DEMORA_MS * 2)

    expect(demorou()).toBe(false)
  })

  it('o aviso some quando a resposta chega', async () => {
    const esperando = ref(true)
    const demorou = montar(esperando)
    vi.advanceTimersByTime(DEMORA_MS)
    expect(demorou()).toBe(true)

    esperando.value = false
    await nextTick()

    expect(demorou()).toBe(false)
  })
})
