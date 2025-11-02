# Adventure RPG

> Unity 3D 전투 기반 RPG 게임 — 클릭 이동, 전투, 무기 장착, 세이브/로드 시스템까지 완비된 어드벤처.
> A Unity 3D action RPG featuring click-to-move, combat, weapon pickup, and save/load functionality.

<img src="LaserDefenderMain.PNG"/>

<p align="center">
  <a href="#demo">🎮 데모 보기</a> •
  <a href="#features">⚔️ 주요 특징</a> •
  <a href="#tech-stack">🧰 기술 스택</a> •
  <a href="#setup">⚙️ 설치/실행</a> •
  <a href="#screenshots">🖼️ 스크린샷</a>
</p>

<p>
  <img alt="Unity" src="https://img.shields.io/badge/Unity-2022.3_LTS-black?logo=unity"/>
  <img alt="License" src="https://img.shields.io/badge/License-MIT-green"/>
  <img alt="Platform" src="https://img.shields.io/badge/Platform-Windows%20%7C%20macOS-blue"/>
</p>

---

## TL;DR

* **장르**: 3D 액션 RPG
* **엔진**: Unity 2022.3 LTS (URP)
* **역할(Role)**: 기획 100%, 프로그래밍 100%, 아트 20% (Low-poly 리소스 활용)
* **플레이타임**: 약 15분 (기본 퀘스트/전투 시퀀스)

---

## Demo / 데모 보기 {#demo}

* ▶️ **Gameplay Video**: 준비 중
* 📦 **Windows 빌드**: 준비 중

> 플레이어는 마을을 탐험하고, 무기를 습득해 적을 처치하며 경험치를 쌓습니다.

---

## 주요 특징 / Features {#features}

* 🧭 **마우스 클릭 이동 시스템**: NavMesh 기반 경로 탐색 (좌클릭으로 목적지 이동)
* ⚔️ **전투 시스템**: 클릭 타겟 공격, 적 체력 UI 표시
* 🪓 **무기 픽업 및 장착**: 필드의 무기(검, 지팡이 등)를 클릭해 장비 가능
* 💾 **세이브/로드 시스템**: JSON 기반 세이브 파일 관리 (저장/불러오기/삭제)
* 🏠 **마을/필드 구조**: 조명과 물리기반 재질로 구현된 마을과 하천 환경

---

## 기술 스택 / Tech Stack {#tech-stack}

**엔진**: Unity 2022.3 LTS (URP)

**언어**: C#

**툴**: Rider / Visual Studio Code / Git / Blender / Audacity

**주요 시스템**:

* **NavMesh Agent**: 이동 및 추적 AI
* **Animator Controller**: 공격/대기/피격 애니메이션 전환
* **Cinemachine**: 3인칭 카메라 추적
* **ScriptableObject**: 캐릭터/무기 스탯 관리
* **SavingWrapper / Fader**: 씬 전환 및 세이브 관리

---

## 프로젝트 구조 / Architecture

```
Assets/
  Scripts/
    Player/
      PlayerController.cs
      WeaponPickup.cs
    Enemy/
      EnemyAIController.cs
    Systems/
      SaveSystem.cs
      GameManager.cs
      UIManager.cs
```

**주요 설계 패턴**:

* 이벤트 기반 전투 처리 (OnAttack, OnDeath)
* 세이브 데이터 직렬화(JSON SaveData)
* 단방향 의존성 구조: Player → Systems

---

## 설치 및 실행 / Setup {#setup}

1. 저장소 클론:

```bash
git clone https://github.com/<YOUR_ID>/AdventureRPG.git
```

2. Unity Hub에서 `AdventureRPG` 프로젝트 열기
3. 패키지 복구 (Package Manager)
4. `Assets/Scenes/MainScene.unity` 실행 후 ▶️ Play

---

## 조작법 / Controls

| 동작    | 조작                |
| ----- | ----------------- |
| 이동    | 마우스 좌클릭           |
| 공격    | 마우스 좌클릭 (적 타겟 시)  |
| 무기 줍기 | 무기 클릭             |
| 저장    | ESC → Save        |
| 불러오기  | ESC → Load        |
| 삭제    | ESC → Delete Save |

---

## 스크린샷 / Screenshots {#screenshots}

<p align="center">
  <img src="./docs/adventure_rpg_scene.png" width="70%"/>
</p>

> 플레이어는 마을을 탐험하며 무기를 수집하고 전투를 통해 경험치를 쌓습니다.

---

## 향후 계획 / Roadmap

* [ ] 퀘스트 시스템 추가 (NPC 대화 및 목표 추적)
* [ ] 보스 몬스터 추가 및 AI 개선
* [ ] 인벤토리 및 장비창 UI 구현
* [ ] 저장 슬롯 시스템 확장

---

## 제작자 / Credits

* 기획·개발: 나현 (Nayun)
* 아트 리소스: Lowpoly Village Pack (Unity Asset Store)
* 사운드: FreeSound.org / 자체 믹싱

---

## 라이선스 / License

* 소스코드: MIT License
* 사용 애셋: 각 저작권자 표시 (비상업적 포트폴리오 용도)

---

## 연락처 / Contact

* 이메일: [your.email@example.com](mailto:your.email@example.com)
* 포트폴리오: [https://your-portfolio.site](https://your-portfolio.site)
