import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { chaveAula, progressoNoNavegador } from '../src/progresso/progresso'

beforeEach(() => localStorage.clear())
afterEach(() => vi.restoreAllMocks())

describe('progresso do visitante (localStorage)', () => {
  it('curso nunca aberto começa sem aulas concluídas', async () => {
    expect(await progressoNoNavegador.abrir('solda')).toEqual([])
  })

  it('guarda a aula concluída por curso, como "modulo.aula"', async () => {
    await progressoNoNavegador.concluir('solda', 1, 2)
    await progressoNoNavegador.concluir('robotica', 3, 1)

    expect(await progressoNoNavegador.abrir('solda')).toEqual(['1.2'])
    expect(await progressoNoNavegador.abrir('robotica')).toEqual(['3.1'])
  })

  it('concluir a mesma aula duas vezes não duplica', async () => {
    await progressoNoNavegador.concluir('solda', 1, 1)
    await progressoNoNavegador.concluir('solda', 1, 1)

    expect(await progressoNoNavegador.abrir('solda')).toEqual(['1.1'])
  })

  it('valor corrompido no navegador é ignorado em vez de quebrar a tela', async () => {
    localStorage.setItem('sesi:progresso:solda', '{nao é json')
    expect(await progressoNoNavegador.abrir('solda')).toEqual([])

    localStorage.setItem('sesi:progresso:solda', JSON.stringify(['1.1', 42, null]))
    expect(await progressoNoNavegador.abrir('solda')).toEqual(['1.1'])
  })

  it('navegador que recusa gravar (aba anônima, cota) não derruba a conclusão', async () => {
    vi.spyOn(Storage.prototype, 'setItem').mockImplementation(() => {
      throw new DOMException('cheio', 'QuotaExceededError')
    })

    await expect(progressoNoNavegador.concluir('solda', 1, 1)).resolves.toBeUndefined()
  })

  it('chaveAula junta módulo e aula', () => {
    expect(chaveAula(2, 5)).toBe('2.5')
  })
})
