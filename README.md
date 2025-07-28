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
- TableManager를 통해 데이터 접근 및 읽기

```mermaid
sequenceDiagram
    participant GoogleSpreadsheet as G-Sheet
    participant ExporterTool as Export Tool
    participant LocalJSON as JSON Files
    participant UnityEditor as Unity Editor
    participant TableManager as TableManager
    participant GameLogic as Game Logic

    G-Sheet->>ExporterTool: 데이터 업데이트
    ExporterTool->>LocalJSON: 각 테이블별 JSON 파일 생성 (e.g., EntityTable.json)

    UnityEditor->>LocalJSON: JSON 파일 포함 (StreamingAssets)

    activate TableManager
    GameLogic->>TableManager: OnLoadGameAction() 호출
    TableManager->>TableManager: 싱글턴 인스턴스 초기화
    TableManager->>TableManager: LoadAllTables() 호출

    loop 각 데이터 테이블 (e.g., Entity, Champion, Skill)
        Note right of TableManager: **[강조 1] 플랫폼별 파일 로딩 분리**
        TableManager->>LocalJSON: JSON 파일 로드 요청 (e.g., "EntityTable.json")
        alt UNITY_EDITOR || UNITY_STANDALONE
            LocalJSON-->>TableManager: File.ReadAllText()로 파일 내용 반환
        else UNITY_ANDROID || IOS
            LocalJSON-->>TableManager: UnityWebRequest.Get()으로 웹 요청 및 데이터 반환
        end

        TableManager->>TableManager: JsonConvert.DeserializeObject<Dictionary<string, List<T>>> (JSON 파싱)
        TableManager->>TableManager: List<T> 형태로 데이터 추출

        Note right of TableManager: **[강조 2] 제네릭 데이터 변환 및 [강조 3] Dictionary 정리**
        TableManager->>TableManager: `keySelector(T item)`를 사용하여 ID 추출
        TableManager->>TableManager: `Dictionary<int, T>`로 변환 및 저장 (O(1) 조회 구조)
        TableManager->>TableManager: `tableMap` (Dictionary<Type, object>)에 등록

    end
    deactivate TableManager

    GameLogic->>TableManager: FindTableData<T>(id) or FindAllTableData<T>()
    Note right of TableManager: **[강조 2] 제네릭 접근 & [강조 3] Dictionary 빠른 조회**
    TableManager-->>GameLogic: 요청된 테이블 데이터 반환
```


### Google Fit API & Android Native Code(Java)


### AR Foundation

