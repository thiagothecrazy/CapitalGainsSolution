# Capital Gains CLI

## 📌 Objetivo

Aplicação de linha de comando (CLI) para cálculo de imposto sobre ganho de capital em operações de compra e venda de ações.  
Entrada e saída seguem o formato JSON via `stdin` e `stdout`.

---

## 🧠 Decisões Técnicas e Arquiteturais

### 1. ✅ Arquitetura: Hexagonal (Ports & Adapters)

- Separação clara entre domínio, entrada (CLI) e lógica de aplicação.
- `Trade` representa a operação unitária; `Portfolio` mantém o estado da simulação.
- A entrada CLI é desacoplada da lógica de negócio.
- Permite fácil evolução para REST.

---

### 2. ✅ Modelo de Domínio

- `Trade`: representa cada evento de compra ou venda.
- `Portfolio`: calcula o imposto e mantém o estado entre operações.
- `TaxResult`: resultado final com imposto calculado.
- `OperationDto`: estrutura de entrada vinda do JSON.

---

### 3. ✅ Simplicidade vs Extensibilidade

- A solução é minimalista, mas pronta para extensões:
  - Logs estruturados
  - Tracing opcional (`traces/`)
  - Permte evolução para repositórios

---

### 4. ✅ Observabilidade

- Logs via **Serilog** para console
- **OpenTelemetry** habilitado com exportação para console

---

### 5. ❌ Paralelismo/Threads

Não aplicado paralelismo devido à natureza sequencial do `stdin` e custo/benefício baixo.  
A arquitetura permite paralelismo futuro para API batch ou arquivos.

---

## ✅ Testes

| Tipo           | Framework         | Objetivo                                                    |
|----------------|-------------------|--------------------------------------------------------------|
| Unidade        | xUnit + FluentAssertions | Validação de lógica isolada do domínio (`Portfolio`, `Calculator`) |
| Integração     | xUnit             | Execução via `StdInRunner` com simulação de console         |

---

## ▶️ Execução

### CLI

```bash
dotnet run --project CapitalGains.Cli < input.txt
```

### Testes

```bash
dotnet test
```

### Estrutura esperada

- Entrada: várias linhas JSON de operações
- Saída: linhas JSON com imposto (`TaxResult`) correspondente

---

## 🧪 Dependências

- .NET 8
- Serilog
- OpenTelemetry
- xUnit
- FluentAssertions

---

## ⚠️ Notas Finais

- Não usa infraestrutura externa
- Organização pensada para leitura, testes e extensões futuras

