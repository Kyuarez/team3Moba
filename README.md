# 🐾 League of Champions : Moba

## 🎮 개요
<div align="center">
  <img width="812" height="447" alt="Image" src="https://github.com/user-attachments/assets/275ff782-ac89-42c0-aed4-13a2ff05c6fb" />
</div>
Moba 장르의 멀티플레이 게임입니다.

* **프로젝트 이름**: League of Champions 
* **프로젝트 지속기간**: 2025.06.13 ~ 2025.06.27
* **개발 엔진 및 기술**: Unity(Netcode for GameObjects), C#, Google spreadsheet(json Extentions)
* **팀 멤버**: 팀 "동물원" ( 김광석, 정승호, 정보연, 한태규)

---

## 📖 게임 영상
[![Game Demo](https://img.youtube.com/vi/u795ksiAFGA/0.jpg)](https://youtu.be/u795ksiAFGA)

---

## 🕹️ 프로젝트 구현

### Google Spreadsheet 기반 데이터 관리 시스템
게임의 모든 데이터를 Google Spreadsheet에서 중앙 집중식으로 관리하고, 이를 JSON 형태로 추출하여 게임 런타임에 효율적으로 불러와 사용하는 시스템입니다.
- 구글 스프레드 시트에서 Export Json 확장 툴로 json 파일로 변환
- StreamingAsset 폴더에 json들을 각 모델에 맞게 파싱해서 데이터 화
- TableManager를 통해 데이터 접근 및 읽기 (각 시트 별로 Dictionary<key, data> 단위로 저장)

*StreamingAsset폴더에 저장한 이유는 json을 원시 파일로 관리하기 위해서이다.

```mermaid
sequenceDiagram
    participant GameLogic as Game Logic
    participant TableManager as TableManager
    participant StreamingAssets as StreamingAssets JSON
    participant InGameData as In-Game Data (Dictionary)

    activate TableManager
    GameLogic->>TableManager: OnLoadGameAction() 호출
    TableManager->>TableManager: 싱글턴 인스턴스 초기화
    TableManager->>TableManager: LoadAllTables() 호출

    loop 각 데이터 테이블 (e.g., "EntityTable.json", "ChampionTable.json"...)
        Note right of TableManager: **플랫폼별 파일 로딩 분리**
        TableManager->>StreamingAssets: JSON 파일 로드 요청
        alt UNITY_EDITOR || UNITY_STANDALONE
            StreamingAssets-->>TableManager: File.ReadAllText()로 파일 내용 반환
        else UNITY_ANDROID || IOS
            StreamingAssets-->>TableManager: UnityWebRequest.Get()으로 웹 요청 및 데이터 반환
        end

        TableManager->>TableManager: JsonConvert.DeserializeObject<Dictionary<string, List<T>>> (JSON 파싱)
        TableManager->>TableManager: List<T> 형태로 데이터 추출

        Note right of TableManager: **[강조 2] 제네릭 데이터 변환 & [강조 3] Dictionary 정리**
        TableManager->>TableManager: `keySelector(T item)`를 사용하여 각 데이터의 고유 ID 추출
        TableManager->>InGameData: `Dictionary<int, T>`로 변환 후 데이터 저장
        TableManager->>InGameData: `tableMap` (Dictionary<Type, object>)에 등록 (타입별 관리)
    end
    deactivate TableManager

    GameLogic->>TableManager: FindTableData<T>(id) 또는 FindAllTableData<T>()
    Note right of InGameData: **제네릭 접근 & Dictionary 빠른 조회**
    InGameData-->>GameLogic: 요청된 테이블 데이터 반환 (O(1) 속도)
```


### Skill System
스킬 시스템은 데이터 기반으로 정의된 스킬을 플레이어 입력에 따라 실행하고, 다양한 스킬 액션 타입(Launch, Buff 등)을 유연하게 처리하는 구조입니다.
개별 스킬 Act하는 부분을 전략 패턴(특정 작업을 클래스 화해서 런타임에 변환)적으로 구현했습니다.

```mermaid
classDiagram
    direction LR
    class SkillManager {
        - reservationSkill: SkillTable
        - skillActorDict: Dictionary<SkillActionType, SkillActor>
        + Awake()
        + ExecuteSkill(caster: GameEntity)
        + ExecuteSkill(caster: GameEntity, hit: RaycastHit)
        - Execute(caster: GameEntity, target: GameEntity)
        + SetReservationSkill(skill: SkillTable)
        + ResetReservationSkill()
    }

    class SkillActor {
        <<abstract>>
        + Execute(data: SkillTable, caster: GameEntity, target: GameEntity)
    }

    class LaunchSkillActor {
        + Execute(data: SkillTable, caster: GameEntity, target: GameEntity)
    }

    class SkillTable {
        + id: int
        + skill_name: string
        + cool_time: float
        + excute_type: SkillExecuteType
        + action_type: SkillActionType
        // ... more properties
    }

    class GameEntity {
        // ... properties
        + ServerShootRpc(targetId, skillName, damage, ...)
        + GetTeam(): Team
        + IsInvincible(): bool
    }

    class Champion {
        // ... properties
        + PlayerCoolTime: PlayerCoolTime
    }

    enum SkillExecuteType {
        Immediately  // <-- 이 부분!
        NoneTarget
        SetTarget
    }

    enum SkillActionType {
        Buff
        Launch
        // ... other types
    }

    SkillManager --o SkillTable : uses (reservationSkill)
    SkillManager ..> SkillActor : instantiates/uses (skillActorDict)
    SkillActor <|-- LaunchSkillActor
    SkillManager --|> MonoSingleton
    LaunchSkillActor ..> GameEntity : calls ServerShootRpc
    SkillManager ..> GameEntity : casts/uses
    GameEntity <|-- Champion
    SkillManager ..> SoundManager : plays SFX
    SkillManager ..> PlayerCoolTime : sets cooltime
    SkillTable ..> SkillExecuteType
    SkillTable ..> SkillActionType
```



