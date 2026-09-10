# Space Invaders — Unity (FEI)

Projeto completo em C# implementando os requisitos pedidos:

- [x] Jogador com 3 vidas
- [x] Tiros do jogador/inimigos
- [x] Naves comuns (movimento em passos + tiro aleatório, conforme o slide)
- [x] Nave mãe (aparece a cada 30–50s, cruza a tela)
- [x] Incremento de velocidade (aumenta a cada nave destruída)
- [x] Pontuação (10 / 20 / 30 / 50 pontos por tipo)
- [x] Cena de Vitória
- [x] Cena de Derrota

Os scripts estão em `Assets/Scripts/`. Você só precisa importá-los num
projeto Unity 2D novo e montar as cenas/prefabs seguindo os passos abaixo
(mesma lógica do slide "Como fazer...?").

---

## 1. Criar o projeto e importar o sprite sheet

1. Crie um projeto Unity **2D (URP ou Built-in, tanto faz)**.
2. Copie a pasta `Assets/Scripts` deste pacote para dentro do seu projeto.
3. Arraste o sprite sheet (`SpaceInvaders.png`, o mesmo do slide) para
   `Assets/Sprites/`.
4. Selecione o sprite sheet → aba **Inspector** → **Sprite Mode: Multiple**.
5. Clique em **Sprite Editor** → **Slice** → **Grid By Cell Size**:
   - Pixel Size: X:102 Y:80
   - Offset: X:5 Y:25
   - Padding: X:8 Y:40
   - **Apply**

Isso recria a mesma grade A–Z mostrada no slide.

Sugestão de uso dos sprites (ajuste como preferir):

| Sprite | Uso sugerido |
|---|---|
| D, E | Nave comum tipo 1 (10 pts) |
| F, G | Nave comum tipo 2 (20 pts) |
| J, K, L | Nave comum tipo 3 (30 pts) |
| U, V | Nave mãe / chefe (50 pts) |
| W | Nave do jogador (canhão) |
| Y | Míssil (jogador e inimigo — pode reescalar/rotacionar) |
| Z | Efeito de explosão (opcional, ao destruir) |

---

## 2. Tags e Layers

Crie estas **Tags** (Project Settings → Tags and Layers):

- `Player`
- `PlayerMissile`
- `EnemyMissile`
- `Enemy`
- `MotherShip`
- `Wall`

## 3. Prefabs

### Player
1. Crie um GameObject com o sprite `W`.
2. Adicione **Rigidbody2D** (Body Type: `Kinematic`, Gravity Scale: 0).
3. Adicione **Box Collider 2D** com `Is Trigger` marcado.
4. Tag: `Player`.
5. Adicione o script `PlayerController`.
6. Crie um filho vazio `FirePoint` posicionado no topo do sprite e
   arraste-o no campo `Fire Point` do script.
7. Posicione o jogador na base da tela (ex: `Y = -4`).

### Míssil do jogador
1. GameObject com sprite `Y`, **Rigidbody2D** (Kinematic, Gravity 0),
   **Box Collider 2D** (`Is Trigger`).
2. Tag: `PlayerMissile`.
3. Script `Missile` → `Owner = Player`.
4. Vire prefab (`Assets/Prefabs/PlayerMissile`) e arraste no
   `Missile Prefab` do `PlayerController`.

### Míssil inimigo
Igual ao anterior, mas Tag `EnemyMissile` e `Owner = Enemy` no script
`Missile`. Prefab: `Assets/Prefabs/EnemyMissile`.

### Naves comuns (3 tipos)
Para cada tipo (10 / 20 / 30 pontos):
1. GameObject com o sprite correspondente.
2. **Rigidbody2D** (Kinematic, Gravity 0), **Box Collider 2D** (`Is Trigger`).
3. Tag: `Enemy`.
4. Script `EnemyUnit`:
   - `Point Value`: 10, 20 ou 30 conforme o tipo.
   - `Enemy Missile Prefab`: arraste o prefab `EnemyMissile`.
5. Vire prefab (`Assets/Prefabs/Enemy_Tipo1`, `Enemy_Tipo2`, `Enemy_Tipo3`).

### Nave mãe
1. GameObject com sprite `U`/`V`.
2. **Rigidbody2D** (Kinematic, Gravity 0), **Box Collider 2D** (`Is Trigger`).
3. Tag: `MotherShip`.
4. Script `MotherShip` (`Point Value = 50`).
5. Vire prefab `Assets/Prefabs/MotherShip`.

### Paredes (limites da tela)
Crie 3 objetos vazios com **Box Collider 2D** (`Is Trigger`) nas laterais
e no topo do ambiente, todos com Tag `Wall`. Isso é o que faz os mísseis
serem destruídos ao saírem da tela (veja `Missile.cs`).

---

## 4. Montando a cena principal (`MainScene`)

1. Crie um GameObject vazio `GameManager` e adicione o script `GameManager`.
2. Crie um GameObject vazio `EnemySpawner`, adicione o script `EnemySpawner`
   e configure o array `Rows` com os 3 prefabs de naves comuns e quantas
   colunas cada linha deve ter (ex.: 11 colunas por linha, várias linhas).
3. Crie um GameObject vazio `MotherShipSpawner`, adicione o script
   `MotherShipSpawner` e arraste o prefab da nave mãe.
4. Arraste o prefab do `Player` para a cena.
5. Crie um `Canvas` com dois `Text` (Score e Vidas), adicione o script
   `HUDController` e arraste os textos correspondentes.

## 5. Cenas de Vitória e Derrota

1. Crie duas novas cenas: `VictoryScene` e `DefeatScene`.
2. Em cada uma, adicione um `Canvas` com:
   - Um `Text` para "VOCÊ VENCEU!" / "VOCÊ PERDEU!" (fixo).
   - Um `Text` para a pontuação final (arraste no `Final Score Text`).
   - Um `Button` "Jogar Novamente" ligado ao método
     `EndSceneController.RestartGame()`.
3. Adicione o script `EndSceneController` ao Canvas em ambas as cenas.
4. Em **Build Settings**, adicione as 3 cenas nesta ordem:
   `MainScene`, `VictoryScene`, `DefeatScene` (os nomes usados nos
   campos `Victory Scene Name` / `Defeat Scene Name` / `Game Scene Name`
   dos scripts devem bater exatamente com os nomes das suas cenas).

---

## 6. Como cada requisito foi implementado

| Requisito | Script |
|---|---|
| Vidas do jogador (3) | `GameManager` (`startingLives`) + `PlayerController` |
| Tiros do jogador/inimigos | `Missile` (com `Owner`) |
| Naves comuns | `EnemyUnit` (movimento em passos: 10 direita, 10 esquerda, desce, repete — igual ao slide) |
| Nave mãe | `MotherShip` + `MotherShipSpawner` (spawn aleatório 30–50s) |
| Incremento de velocidade | `GameManager.EnemySpeedMultiplier`, aumenta a cada `OnEnemyDestroyed` |
| Pontuação | `GameManager.AddScore`, valores por tipo em `EnemyUnit.pointValue` / `MotherShip.pointValue` |
| Cena de vitória | `GameManager.Victory()` → carrega `VictoryScene` quando todas as naves comuns são destruídas |
| Cena de derrota | `GameManager.Defeat()` → carrega `DefeatScene` se as vidas chegarem a 0 ou uma nave tocar a base |

## 7. Extensões possíveis (para nota extra)

- Múltiplos níveis: ao vencer, em vez de ir direto para `VictoryScene`,
  recarregar `MainScene` com uma leva nova e mais rápida, só indo para a
  vitória após N níveis.
- Efeito de explosão usando o sprite `Z` (Instantiate + Destroy após alguns
  frames em `OnEnemyDestroyed`).
- Sons de tiro/explosão e trilha sonora.
- Menu inicial antes da `MainScene`.
