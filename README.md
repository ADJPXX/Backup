# Backup

Aplicativo de console para Windows que copia e restaura arquivos de configuração, documentos, scripts e projetos entre pastas locais. Também pode copiar pastas para uma unidade sincronizada com a nuvem, criar tarefas no Agendador de Tarefas, criar diretórios e instalar ou atualizar programas com `winget`.

O projeto é personalizado para um computador com caminhos fixos em `D:\`, `E:\` e, por padrão, `G:\Meu Drive`. Ele não é um serviço de backup genérico: antes de usá-lo em outra máquina, revise os caminhos, a configuração e os efeitos descritos neste README.

> **Atenção a operações de movimentação:** a opção de backup para o drive tenta copiar `Downloads\TUDO` para `D:\Backups\TUDO` e depois usa `/MOVE` para movê-la a `D:\TUDO`. A movimentação é tentada mesmo se a primeira cópia reportar falha. A restauração também move `D:\TUDO` para Downloads e move os vídeos de `D:\Vídeos gravados` para a pasta de vídeos do usuário. Confirme esses caminhos e tenha uma cópia independente dos arquivos importantes antes de executar.

## Requisitos e ambiente esperado

- Windows; o programa depende de APIs do Windows, Registro, Agendador de Tarefas, `powercfg` e `robocopy`.
- .NET 10 SDK para compilar. A publicação do workflow é autocontida para `win-x64` e não exige instalar o runtime do .NET na máquina de destino.
- Execução como administrador. Se iniciado sem ele, o programa pede elevação pelo UAC e encerra a instância atual.
- `D:\` para os dados de backup e `E:\` para o Dev Drive e os repositórios. O aplicativo verifica especificamente a existência de `E:\`.
- `winget` para instalar e atualizar pacotes.
- Para o backup em nuvem padrão, uma pasta local acessível em `G:\Meu Drive\BackupCloud\`, normalmente fornecida pelo Google Drive para computador. O aplicativo copia para esse caminho local; a sincronização remota é responsabilidade do cliente instalado.

As pastas de perfil do Windows são descobertas em tempo de execução usando `Environment.SpecialFolder` para Documentos, perfil do usuário, `AppData`, `LocalAppData` e Arquivos de Programas (x86). Portanto, o caminho de Documentos pode ser redirecionado pelo Windows, embora vários destinos de backup permaneçam fixos em `D:\`.

## Compilar e publicar

Na raiz do repositório:

```powershell
dotnet restore .\Backup.sln
dotnet build .\Backup.sln -c Release
dotnet publish .\Backup\Backup.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=false `
  -o .\publish
```

Execute `publish\Backup.exe` como administrador. O arquivo de configuração e o log são criados ao lado do executável publicado.

O workflow `.github/workflows/release.yml` é iniciado manualmente (`workflow_dispatch`). Ele restaura e publica a versão `win-x64` autocontida em arquivo único e cria uma Release no GitHub com `Backup.exe`.

## Inicialização e menu

Ao iniciar, o programa:

1. Solicita elevação administrativa se necessário.
2. Lê `BackupConfig.json` ao lado do executável; se o arquivo não existir, grava nele uma configuração padrão.
3. Garante a existência de `BackupLog.log` no mesmo diretório.
4. Verifica as tarefas configuradas. Tarefas inexistentes são criadas no Agendador para iniciar no logon do usuário, com atraso em segundos e nível de execução mais alto. Se algum executável configurado não for encontrado, solicita uma tentativa de restauração de `Documentos\SCRIPTS` a partir de `D:\SCRIPTS` e repete a verificação; o prompt pode continuar aparecendo até todos os executáveis existirem.
5. Altera três valores de atalho de idioma no Registro do usuário, ativa o esquema de energia `SCHEME_MIN` e desativa os tempos limite de tela e suspensão **na alimentação AC**.
6. Verifica `E:\` e abre o menu interativo.

As alterações de Registro e energia acontecem em toda inicialização, antes da seleção no menu. As tarefas existentes não são atualizadas automaticamente quando seus valores mudam no JSON; a criação só ocorre para tarefas que ainda não existem.

| Opção | Ação |
| --- | --- |
| `0` | Encerra o programa. |
| `1` | Copia configurações e dados selecionados para os destinos de backup locais. |
| `2` | Copia as pastas de `D:\` listadas em `CloudBackupFolders` para `CloudBackupPath`. |
| `3` | Restaura os conjuntos de dados implementados no código a partir dos destinos locais. |
| `4` | Cria a pasta de vídeos gravados e as pastas listadas em `FoldersToCreate` sob `E:\Repositories`. |
| `5` | Desinstala `Microsoft.OneDrive` e instala os pacotes listados em `Apps` com `winget`. |
| `6` | Executa `winget upgrade --include-unknown --all`. |
| `7` | Abre, no navegador padrão, os links listados em `Links`. |

Todas as opções diferentes de sair passam primeiro pela verificação de `E:\`. Assim, a ausência do Dev Drive impede a execução das ações do menu, mesmo para nuvem e instalação de pacotes.

## Configuração

O arquivo `BackupConfig.json` é lido do diretório do executável. Na primeira execução, os valores padrão abaixo são gravados automaticamente; edite o JSON e reinicie o programa para aplicar mudanças.

| Campo | Uso |
| --- | --- |
| `Apps` | IDs de pacotes do `winget` que a opção 5 tenta instalar. |
| `Tasks` | Tarefas de logon. Cada item tem `Name`, `ExecutablePath` e `Delay`. |
| `BackupFolders` | Subpastas de Documentos copiadas para `D:\Backups\`. |
| `CloudBackupFolders` | Pastas de primeiro nível em `D:\` copiadas para a pasta de nuvem. |
| `CloudBackupPath` | Raiz local do destino em nuvem. |
| `ExcludedFolders` | Nomes de diretórios excluídos em algumas cópias com `robocopy /XD`. |
| `FoldersToCreate` | Subpastas criadas dentro de `E:\Repositories`. |
| `Links` | URLs abertas pela opção 7. |

Exemplo de uma tarefa:

```json
{
  "Name": "TempCleaner",
  "ExecutablePath": "TempCleaner\\TempCleaner.exe",
  "Delay": 10
}
```

`ExecutablePath` é relativo a `Documentos\SCRIPTS`; no exemplo, o arquivo esperado é `Documentos\SCRIPTS\TempCleaner\TempCleaner.exe`. O atraso é em segundos e valores menores que 5 são ajustados para 5. As tarefas são registradas no Agendador para iniciar no logon do usuário com nível mais alto.

### Valores padrão gerados

- `BackupFolders`: `AC Content Manager`, `Assetto Corsa`, `Assetto Corsa Competizione`, `Automobilista 2`, `iRacing`, `My Games`, `PCSX2` e `RaceLabApps`.
- `CloudBackupFolders`: `Backups`, `Book do globis`, `Codigos`, `Contratos apartamentos`, `Fotos Steam`, `Instaladores`, `Jogos e emuladores`, `SCRIPTS`, `Vídeos` e `Wallpapers`.
- `CloudBackupPath`: `G:\Meu Drive\BackupCloud\`.
- `ExcludedFolders`: `log`, `cache`, `replay`, `logs`, `caches` e `replays`.
- `FoldersToCreate`: `C`, `C#` e `Python`, criadas como `E:\Repositories\C`, `E:\Repositories\C#` e `E:\Repositories\Python`.
- `Tasks`: `AC_Controller` (30 s), `TempCleaner` (10 s), `CloudBackup` (30 s), `MyCalendar` (5 s), `InstantReplayChecker` (5 s), `PS3DiscordRichPresence` (5 s) e `iRacingWheelSync` (5 s). Os caminhos dos executáveis vêm definidos em `InitializerService`.
- `Apps`: `AgileBits.1Password`, `Valve.Steam`, `Google.GoogleDrive`, `9N0866FS04W8`, `Logitech.GHUB`, `Google.Chrome`, `Parsec.Parsec`, `Discord.Discord`, `OBSProject.OBSStudio`, `Guru3D.Afterburner`, `Guru3D.RTSS`, `Microsoft.DotNet.SDK.10`, `Git.Git`, `Axosoft.GitKraken` e `JetBrains.Toolbox`.

Se `BackupConfig.json` estiver malformado, a leitura registra a exceção no console, mas o programa não interrompe a inicialização nem valida os campos. Nesse caso, as listas podem permanecer vazias e tarefas, cópias ou links podem não funcionar como esperado.

## O que é copiado

Os caminhos abaixo refletem o código atual. `{Documentos}`, `{AppData}`, `{LocalAppData}` e `{ProgramFilesX86}` representam as pastas correspondentes do usuário ou do Windows em execução.

### Backup local (`D:\Backups` e outros destinos em `D:\`)

| Origem | Destino | Observações |
| --- | --- | --- |
| `{Documentos}\<nome>` para cada item de `BackupFolders` | `D:\Backups\<nome>` | Cópia recursiva. A entrada `My Games` é especial: o código copia `{Documentos}\My Games\Rocket League` para `D:\Backups\My Games\Rocket League`, não toda a pasta `My Games`. |
| `{Documentos}\SCRIPTS` | `D:\SCRIPTS` | Cópia recursiva; esse destino também é usado para recuperar scripts necessários às tarefas agendadas. |
| Cada subpasta imediata de `E:\Repositories` | `D:\Codigos\<nome>` | Cópia recursiva. O código também copia `.github` e `.gitignore` de `E:\Repositories\C#` para `D:\Codigos\C#`. |
| `{AppData}\Blackmagic Design` | `D:\Backups\DaVinci Resolve\Blackmagic Design` | Preferências do DaVinci Resolve. |
| `{AppData}\obs-studio` | `D:\Backups\obs-studio` | Configurações do OBS. |
| `{LocalAppData}\DuckStation` | `D:\Backups\DuckStation` | Dados do DuckStation. |
| `{ProgramFilesX86}\Steam\steamapps\common\Counter-Strike Global Offensive\game\csgo\cfg` | `D:\Backups\CS\CFGS` | Apenas arquivos `*.cfg`. |
| `{LocalAppData}\iRacing\_useSteam.txt` | `D:\Backups\iRacingLocal` | Apenas `_useSteam.txt`. |
| `{ProgramFilesX86}\Steam\steamapps\common\Le Mans Ultimate\UserData` | `D:\Backups\LMU\UserData` | Cópia recursiva com exclusões configuradas. |
| `{AppData}\racelabapps` | `D:\Backups\RaceLabAppData\racelabapps` | Apenas arquivos `*.json`, com exclusões configuradas. |
| `{AppData}\Battlestate Games\Escape from Tarkov\Settings` | `D:\Backups\Tarkov` | Apenas arquivos `*.ini`. |
| `{ProgramFilesX86}\MSI Afterburner\Profiles` | `D:\Backups\Afterburner\Profiles` | Cópia recursiva com exclusões configuradas. |
| Perfis e plugins do RTSS em `{ProgramFilesX86}\RivaTuner Statistics Server` | `D:\Backups\Riva\Profiles` e `D:\Backups\Riva\Plugins` | Cópias recursivas com exclusões configuradas. |
| `{perfil}\Downloads\TUDO` | Primeiro `D:\Backups\TUDO`, depois `D:\TUDO` | Tenta fazer uma cópia e, em seguida, move a origem para `D:\TUDO`, mesmo se a primeira cópia reportar falha. |
| `{perfil}\Videos\Vídeos gravados` | `D:\Vídeos gravados` | Pede confirmação se encontrar ao menos uma subpasta imediata e arquivos com tamanho total maior que zero. Soma todos os arquivos recursivamente; não filtra por extensão de vídeo. |

Para a maioria das cópias recursivas, o código usa `/E /COPY:DAT /R:3 /W:5`: inclui subpastas vazias, copia dados, atributos e datas, e tenta novamente até três vezes com espera de cinco segundos. Algumas cópias de pastas aplicam `/XD` com `ExcludedFolders`; a lista não é aplicada uniformemente a todas as operações. A cópia não espelha o destino com `/MIR`, então arquivos extras no destino não são removidos.

### Backup em nuvem

Para cada nome de `CloudBackupFolders`, a origem é `D:\<nome>` e o destino é `<CloudBackupPath>\<nome>`. O aplicativo verifica se a raiz de nuvem existe e pula as pastas de origem inexistentes. Esse fluxo também usa `robocopy`; não chama uma API de armazenamento em nuvem.

## Restauração

A opção 3 copia `D:\Backups\<nome>` de volta para `{Documentos}\<nome>` para as pastas configuradas e inverte a direção para scripts e configurações de aplicativos. Perfis de Counter-Strike, iRacing, Le Mans Ultimate, RaceLab, Tarkov, MSI Afterburner e RTSS são copiados dos destinos `D:\` para os caminhos locais correspondentes.

Há duas exceções relevantes:

- Para repositórios, a restauração não copia todas as pastas de `D:\Codigos` para `E:\Repositories`. Ela restaura `publish.txt`, `.gitignore` e `.github` para `E:\Repositories\C#` quando esses itens existem no backup.
- `D:\TUDO` é movida para `{perfil}\Downloads\TUDO`, e `D:\Vídeos gravados` é movida para `{perfil}\Videos\Vídeos gravados`. Como usam `/MOVE`, os arquivos transferidos são removidos da origem quando a movimentação é concluída.

A restauração não apaga arquivos extras dos destinos por meio de espelhamento. Revise o conteúdo de origem e destino antes de restaurar; cópias podem sobrescrever arquivos correspondentes conforme as regras do `robocopy`.

## Registro e dependências

- `BackupLog.log`, ao lado do executável, recebe cabeçalhos com data e hora e mensagens de cada execução de backup ou restauração. A opção de nuvem também registra sua execução.
- `RobocopyService` traduz o código de saída do processo em uma mensagem, mas os fluxos de backup continuam e podem terminar com “concluído” mesmo se uma cópia individual tiver falhado.
- A saída redirecionada pelo código é apenas `stderr`; a saída padrão do `robocopy` não é anexada ao log pelo serviço.
- A opção 5 tenta desinstalar `Microsoft.OneDrive` antes de instalar os IDs de `Apps`. As chamadas de `winget` são aguardadas, mas seus códigos de saída não são usados para confirmar sucesso. A mensagem final, portanto, não garante que todos os pacotes foram instalados.
- O projeto contém um único aplicativo de console e não inclui um projeto de testes automatizados.

## Organização do código

```text
Backup.sln
Backup/
  Program.cs                 Inicialização e sequência principal
  Models/
    Config.cs                Configuração carregada em memória
    TaskConfig.cs            Modelo de tarefa agendada
  Services/
    InitializerService.cs    JSON, log e elevação administrativa
    MenuService.cs           Menu interativo
    PathsService.cs          Caminhos locais e letras fixas
    DriveBackupService.cs    Cópias locais para os destinos de backup
    CloudBackupService.cs    Cópia para pasta sincronizada com nuvem
    RestoreBackupService.cs  Cópias e movimentações de restauração
    RobocopyService.cs       Execução do robocopy e interpretação de status
    SchedulerService.cs      Tarefas no Agendador do Windows
    WingetService.cs         Instalação e atualização de pacotes
    LogService.cs            Registro de execuções
    DirectoryService.cs      Criação de diretórios e verificação de vídeos
    PowerPlanService.cs      Ajustes de energia
    LanguageLayoutService.cs Ajustes de atalhos no Registro do usuário
    DriveService.cs          Verificação do Dev Drive
    BrowserService.cs        Abertura dos links configurados
    ConsoleService.cs        Leitura de valores do menu
```

O ponto de entrada está em `Program.Main`. A maior parte dos serviços é estática e usa a configuração e os caminhos globais definidos em `Config` e `PathsService`.

## Análise técnica: limites conhecidos

1. **A inicialização tem efeitos no sistema antes do menu.** Além de criar tarefas configuradas com nível mais alto, muda valores do Registro do usuário e as configurações de energia AC. Isso ocorre ao abrir o aplicativo, não apenas ao escolher uma opção de backup.
2. **O nome “backup” não significa somente cópia.** A pasta `Downloads\TUDO` é movida para `D:\TUDO` mesmo se a tentativa de cópia prévia para `D:\Backups\TUDO` reportar falha; a restauração move `D:\TUDO` de volta. A restauração de vídeos também move os arquivos de `D:\Vídeos gravados`. Verifique esses caminhos antes de usar.
3. **O resultado global não indica confiavelmente falhas parciais.** Os serviços registram o texto do status do `robocopy`, mas não tratam códigos de falha como erro geral nem impedem que a mensagem “concluído” seja exibida. Consulte o log e os caminhos de destino após cada execução.
4. **As tarefas são criadas somente quando faltam.** Alterar o nome, executável, atraso ou outras configurações de uma tarefa existente no JSON não atualiza a tarefa já registrada no Windows.
5. **A configuração não passa por validação centralizada.** Erros de leitura do JSON são capturados e a execução continua; caminhos e listas inconsistentes podem causar operações parciais ou ausência de tarefas.
6. **A cópia não preserva permissões NTFS.** `/COPY:DAT` copia dados, atributos e datas, mas não ACLs, proprietário ou auditoria de segurança. O uso de `/E`, em vez de `/MIR`, também não remove sobras do destino.
7. **A instalação de pacotes pode desinstalar o OneDrive.** Esse comportamento está na opção 5 e ocorre antes das instalações configuradas.
8. **O backup de repositórios e a restauração são assimétricos.** O backup copia cada repositório de `E:\Repositories` para `D:\Codigos`, mas a restauração implementada devolve apenas `publish.txt`, `.gitignore` e `.github` do repositório `C#`.
9. **A verificação do Dev Drive é uma dependência do menu inteiro.** Mesmo a opção de cópia em nuvem e as opções de `winget` passam pela verificação de `E:\` antes de executar.
10. **A criação de tarefas não confirma o registro.** `CreateTask` captura exceções internamente; por isso, `CheckTasks` pode prosseguir sem verificar se a tarefa foi registrada com sucesso.

Esses pontos são observações do comportamento implementado, não garantias de execução em uma máquina específica. As operações dependem da existência dos drives, pastas, aplicativos e permissões esperados.
