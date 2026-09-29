<div align="center">

  <br />
  <h1>📚 Atlas — Book Tracker</h1>
  <p><strong>Mapeia as tuas leituras. Mantém os teus dados onde pertencem: contigo.</strong></p>

  <p>
    An elegant, local-first, privacy-focused book tracker app.
  </p>

  <!-- Badges -->
  <p>
    <img src="https://img.shields.io/badge/Privacy-100%25%20Local-emerald?style=for-the-badge&logo=shield" alt="Privacy First" />
    <img src="https://img.shields.io/badge/Database-Zero%20Server-blue?style=for-the-badge" alt="Zero Server" />
    <img src="https://img.shields.io/badge/License-MIT-purple?style=for-the-badge" alt="MIT License" />
  </p>

</div>

---

## 🌟 Sobre o Atlas

O **Atlas** é um rastreador de leituras minimalista e ultra-rápido desenhado para quem ama ler e valoriza a sua privacidade. 

Diferente de serviços tradicionais de catalogação de livros, o Atlas **não utiliza servidores externos nem bases de dados em nuvem** para guardar a tua biblioteca. Todos os teus dados, notas, históricos de leitura e estatísticas ficam guardados exclusivamente no teu dispositivo.

### 🎯 Porquê o Atlas?

- 🔒 **Privacidade Absoluta:** Os teus dados de leitura pertencem-te a ti. Nenhuma conta necessária, nenhum rastreio.
- ⚡ **Desempenho Instantâneo:** Sem tempos de espera por respostas de servidor. Funciona 100% offline.
- 🪶 **Ultra-Leve:** Consumo de espaço mínimo no dispositivo ao guardar apenas o essencial e otimizar *caching* de capas.
- 📊 **Estatísticas Pessoais:** Visualiza o teu progresso, páginas lidas, metas anuais e hábitos de leitura num painel limpo.

---

## ✨ Funcionalidades Principais

- [x] **Gestão de Biblioteca:** Organiza livros por estados (`Quero Ler`, `A Ler`, `Lido`, `Abandonado`).
- [x] **Pesquisa Rápida:** Integração com APIs abertas (*Open Library / Google Books*) para adicionar livros em segundos.
- [x] **Progresso de Leitura:** Registo diário de páginas lidas e acompanhamento da velocidade de leitura.
- [x] **Notas e Anotações:** Escreve críticas, citações favoritas e reflexões sobre cada obra.
- [x] **Desafios e Metas:** Define a tua meta de leitura anual e acompanha o teu progresso visualmente.
- [x] **Backup e Exportação:** Exporta e importa facilmente toda a tua biblioteca em ficheiro JSON para salvaguarda simples.

---

## 🛠️ Arquitetura Local-First

O Atlas adota uma arquitetura em camadas otimizada para cliente, garantindo que o armazenamento seja eficiente e seguro:

```text
 ┌─────────────────────────────────────────────────────────┐
 │                   Atlas UI / App                        │
 └────────────────────────────┬────────────────────────────┘
                              │
                              ▼
 ┌─────────────────────────────────────────────────────────┐
 │               Lógica de Negócio / Regras                │
 └──────────────┬───────────────────────────┬──────────────┘
                │                           │
                ▼                           ▼
 ┌─────────────────────────────┐ ┌─────────────────────────┐
 │  APIs Públicas de Livros    │ │ Armazenamento Local     │
 │  (Apenas Pesquisa / Capas)  │ │ (MMKV / SQLite / IDB)   │
 └─────────────────────────────┘ └─────────────────────────┘
```

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

Certifica-te de que tens o ambiente configurado para o teu ecossistema de desenvolvimento:

* Node.js (v18+) ou o runtime adequado para a plataforma target.
* Gestor de pacotes (`npm`, `yarn` ou `pnpm`).

### Instalação

1. Clona o repositório:
   ```bash
   git clone https://github.com/teu-usuario/atlas-book-tracker.git
   ```

2. Entra no diretório do projeto:
   ```bash
   cd atlas-book-tracker
   ```

3. Instala as dependências:
   ```bash
   npm install
   ```

4. Inicia a aplicação em modo de desenvolvimento:
   ```bash
   npm run dev
   ```

---

## 📦 Estrutura de Dados (Exemplo)

Para garantir o mínimo consumo de espaço em disco, a estrutura de armazenamento local guarda apenas o essencial:

```json
{
  "bookId": "OL27479W",
  "status": "READING",
  "currentPage": 142,
  "totalPages": 350,
  "rating": 5,
  "startedAt": "2026-02-10",
  "updatedAt": 1774872000
}
```

---

## 🤝 Contribuição

Contribuições são sempre bem-vindas! Se tens ideias para melhorar o Atlas:

1. Faz um *Fork* do projeto
2. Cria uma *Branch* para a tua funcionalidade (`git checkout -b feature/NovaFuncionalidade`)
3. Submete as tuas alterações (`git commit -m 'Adiciona NovaFuncionalidade'`)
4. Faz o *Push* para a Branch (`git push origin feature/NovaFuncionalidade`)
5. Abre um *Pull Request*

---

## 📄 Licença

Este projeto está sob a licença **MIT** — consulta o ficheiro [LICENSE](LICENSE) para mais detalhes.

---

<div align="center">
  <sub>Criado com 💜 para os amantes da leitura.</sub>
</div>