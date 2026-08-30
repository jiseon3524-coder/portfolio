# Ghost Game 프로젝트 기획서

## 1. 프로젝트 개요

### 프로젝트명

**Ghost Game**
※ 가제

### 개발 형태

* 1인 개발
* 포트폴리오용 개인 프로젝트

### 개발 환경

* Engine : Unity 3D
* Language : C#
* Platform : PC
* Version Control : Git / GitHub

### 에셋 제작 방식

캐릭터, 맵, 귀신, 투사체 등의 3D 모델링은 직접 Blender로 제작하지 않고 Unity Asset Store 및 외부 사용 가능 에셋을 활용한다.

직접 구현의 중심은 다음과 같다.

* 플레이어 조작
* 카메라 시스템
* 원거리 전투
* 귀신 AI
* 봉인 및 처형 시스템
* Day 진행 시스템
* 귀신 스폰
* 퀘스트
* 저장 및 이어하기
* UI
* 게임 진행 로직
* 밸런스 조정

---

## 2. 장르 및 게임 컨셉

### 장르

3D 탑다운 액션 / 귀신 퇴치 액션

### 기본 컨셉

플레이어는 귀신 출몰에 대한 의뢰를 받고 산골 마을로 향한다.

플레이어는 마법 투사체를 사용해 귀신의 HP를 감소시키며, 귀신의 HP가 0 이하가 되면 귀신은 바로 죽지 않고 **봉인 상태**에 들어간다.

봉인된 귀신은 일정 시간이 지나면 처형 가능한 상태가 되며, 플레이어가 제한 시간 안에 가까이 접근하여 근거리 처형을 성공해야만 완전히 제거된다.

기본 전투 흐름은 다음과 같다.

```text
공격
↓
Ghost HP 0 이하
↓
자동 봉인
↓
3초 대기
↓
처형 가능 상태
↓
5초 안에 접근
↓
F 근거리 처형
↓
귀신 완전 제거
```

처형에 실패할 경우 귀신은 봉인에서 풀리고 최대 HP가 전부 회복된다.

---

## 3. 프로젝트 개발 범위

현재 개발 목표는 **Stage 1 하나를 처음부터 끝까지 플레이 가능한 상태로 완성하는 것**이다.

Stage 1은 총 3일 동안 진행된다.

* Day 1
* Day 2
* Day 3

각 Day마다 밤에 귀신을 처치하고 오두막으로 돌아오는 구조를 반복한다.

Day 3까지 완료하면 Stage 1이 종료된다.

향후 확장 가능 요소는 존재하지만 현재 개발 범위에는 포함하지 않는다.

예시:

* 추가 스테이지
* 추가 귀신 종류
* 복잡한 보스
* 스킬트리
* 인벤토리
* 장비 시스템
* 아이템 파밍

현재는 Stage 1 완성도에 집중한다.

---

## 4. 게임 시작 흐름

### New Game

메인 메뉴에서 New Game을 선택하면 게임이 시작된다.

기존 저장 데이터가 존재할 경우 바로 새 게임을 시작하지 않는다.

경고창을 표시한다.

> 기존 진행 데이터가 초기화됩니다.
> 새 게임을 시작하시겠습니까?

버튼:

* 예
* 아니오

`예`를 선택하면 기존 Day 저장 데이터를 초기화하고 새로운 게임을 시작한다.

`아니오`를 선택하면 메인 메뉴로 돌아간다.

저장 데이터가 없다면 경고 없이 바로 New Game을 시작할 수 있다.

---

## 5. Continue

저장 데이터가 존재할 경우 메인 메뉴에서 Continue 버튼을 사용할 수 있다.

저장 데이터가 없다면 Continue 버튼은 비활성화한다.

Continue를 선택하면 마지막으로 저장된 Day의 **아침 상태**에서 게임을 시작한다.

예:

```text
Day 2 밤 전투 중 게임 종료
↓
Continue
↓
Day 2 아침부터 다시 시작
```

밤 진행 상황은 저장하지 않는다.

---

## 6. 게임 인트로

New Game에서만 최초 인트로가 실행된다.

Continue에서는 다시 재생하지 않는다.

### 진행 순서

```text
혈서 형태의 의뢰 편지가 화면에 나타남
↓
Space 입력으로 편지를 넘김
↓
편지가 끝나면 주인공 독백 UI 출력
↓
플레이어 조작 가능
↓
작은 도로 맵에서 차량 앞으로 이동
↓
차량 앞에서 Space 입력
↓
화면 Fade Out
↓
약 2초 동안 자동차 시동 및 주행 효과음
↓
산골 마을의 오두막으로 이동
```

주인공 독백 예시:

> 오랜만에 들어온 의뢰다.
> 얼른 오두막으로 이동하자.

---

## 7. Stage 1 기본 배경

Stage 1의 배경은 작은 산골 마을이다.

플레이어는 귀신 출몰 의뢰를 받고 마을에 도착한다.

Stage 1의 중심 공간은 다음과 같다.

* 오두막
* 작은 마을
* 도로
* 건물
* 나무
* 울타리
* 기타 환경 오브젝트

맵은 지나치게 크게 제작하지 않는다.

전투와 탐색이 반복적으로 발생하지만 플레이어가 귀신을 찾기 위해 장시간 이동해야 할 정도의 대형 맵은 사용하지 않는다.

---

## 8. 오두막

오두막은 각 Day의 시작 및 종료 지점이다.

### 아침

플레이어는 오두막에서 하루를 시작한다.

### 밤 시작

오두막 밖으로 나가면 밤이 시작된다.

### 밤 종료

해당 Day의 모든 목표 귀신을 처형해야 다시 오두막에 들어갈 수 있다.

귀신이 남아 있는 상태에서 오두막에 들어가려고 하면 입장을 막는다.

화면에 빨간 안내 문구를 출력한다.

예시:

> 아직 귀신이 남아 있다.

모든 목표를 완료하면:

> 오늘의 귀신을 모두 처치했다.
> 오두막으로 돌아가자.

와 같은 안내를 출력한다.

---

## 9. Day 시스템

Stage 1은 총 3일로 구성된다.

화면 왼쪽 상단에 현재 날짜를 표시한다.

예:

* Day 1
* Day 2
* Day 3

---

## 10. 하루 기본 진행

```text
아침
↓
Player HP 완전 회복
↓
오늘의 목표 표시
↓
오두막 밖으로 이동
↓
Fade Out
↓
밤 Lighting 적용
↓
플레이어 외부 Spawn Point 이동
↓
해당 Day 귀신 생성
↓
Fade In
↓
귀신 전투
↓
모든 목표 귀신 처형
↓
오두막 귀환
↓
Day 종료
↓
다음 날 아침
↓
자동 저장
```

---

## 11. Day별 귀신 구성

Day별 귀신 종류와 숫자는 실제 플레이 테스트 후 최종 결정한다.

### Day 1

기본 귀신 중심.

목표:

* 이동 학습
* 원거리 공격 학습
* 자동 봉인 이해
* 근거리 처형 이해

예상 구성:

* 느린 추적 귀신
* 3~4마리

### Day 2

기본 귀신과 새로운 행동을 가진 귀신 혼합.

예상 구성:

* 기본 근거리 귀신
* 보다 빠른 귀신 또는 다른 행동을 가진 귀신
* 4~6마리

### Day 3

여러 귀신을 동시에 상대하게 한다.

예상 구성:

* 기존 귀신
* 특수 행동 귀신
* 마지막 강한 Elite Ghost
* 약 5~7마리

단순히 HP만 증가시키는 방식보다는 행동 차이를 중심으로 난이도를 높인다.

---

## 12. Stage 1 종료

Day 3에서도 다른 Day와 동일하게 오두막으로 돌아와야 한다.

```text
Day 3 귀신 전멸
↓
오두막 복귀
↓
아침
↓
의뢰 완료 연출 또는 주인공 대사
↓
Stage 1 Clear
```

Stage 1 완료 상태를 저장한다.

Stage Clear 이후 Continue를 눌렀을 때 Day 3 전투가 다시 시작되지 않도록 완료 데이터를 별도로 관리한다.

---

## 13. 플레이어 조작

| 기능     | 키          |
| ------ | ---------- |
| 이동     | WASD       |
| 원거리 공격 | E          |
| 처형     | F          |
| 상호작용   | Space      |
| 게임 방법  | TAB        |
| 화면 회전  | Mouse      |
| 일시정지   | ESC        |
| 대쉬     | 추후 최종 키 결정 |

---

## 14. 플레이어 이동

플레이어는 WASD를 이용하여 이동한다.

이동 방향은 **현재 카메라 화면 방향 기준**으로 계산한다.

즉 마우스로 카메라를 회전시킨 이후에도:

* W = 현재 화면 기준 위쪽
* S = 현재 화면 기준 아래쪽
* A = 현재 화면 기준 왼쪽
* D = 현재 화면 기준 오른쪽

방향으로 이동한다.

플레이어 캐릭터는 현재 이동하는 방향을 바라본다.

플레이어가 이동을 멈추면 마지막으로 바라보던 방향을 유지한다.

---

## 15. 카메라 시스템

게임은 3D 탑다운 시점을 사용한다.

카메라는 플레이어를 따라 이동한다.

단, 플레이어의 회전과 카메라 회전은 독립적이다.

플레이어가 이동하면서 방향을 바꾸더라도 카메라는 자동으로 회전하지 않는다.

화면 방향 변경은 마우스 입력에 의해서만 발생한다.

```text
Player Rotation ≠ Camera Rotation
```

### 카메라 구조

권장 Hierarchy:

```text
Player
├─ Model
├─ ProjectileSpawnPoint
└─ CameraTarget

CameraRig
└─ Main Camera
```

CameraRig는 Player의 CameraTarget을 따라 이동한다.

마우스 입력은 CameraRig의 회전에 사용한다.

플레이어 회전은 CameraRig의 회전에 직접 영향을 주지 않는다.

---

## 16. 대쉬

플레이어는 짧은 시간 빠르게 이동할 수 있다.

대쉬는 무적 회피기가 아니다.

### 특징

* 짧은 시간 이동 속도 증가
* 이동 거리 확보
* 전투 중 위치 조절
* 쿨타임 존재
* 무적 판정 없음

대쉬 중에도 귀신의 공격을 받으면 데미지를 입는다.

정확한 속도, 지속 시간, 쿨타임은 플레이 테스트 후 결정한다.

---

## 17. 기본 원거리 공격

### 공격 키

`E`

E를 누르면 플레이어가 바라보는 방향으로 마법 투사체를 발사한다.

공격 방향은 카메라 방향이 아니라 **Player.forward**를 기준으로 한다.

즉 플레이어 캐릭터가 실제로 바라보고 있는 방향으로 투사체가 발사된다.

---

## 18. Magic Projectile

마법 투사체는 기본 원거리 공격 수단이다.

### 기능

* ProjectileSpawnPoint에서 생성
* Player.forward 방향으로 이동
* 일정 속도로 직선 이동
* Normal 상태의 Ghost와 충돌하면 데미지
* 충돌 후 제거
* 일정 시간이 지나면 자동 제거

### Inspector 조정 값

* Damage
* Speed
* Life Time

ProjectileSpawnPoint Transform을 사용하므로 별도의 Spawn Distance 값은 사용하지 않는다.

---

## 19. Ghost 기본 구조

Ghost는 일반적인 HP 기반 몬스터처럼 HP가 0이 되는 즉시 사망하지 않는다.

Ghost의 HP가 0 이하가 되면 **봉인 상태**로 전환된다.

Ghost의 주요 기능:

* Max HP
* Current HP
* 현재 상태
* 피격
* 자동 봉인
* 봉인 타이머
* 처형 가능 타이머
* 봉인 해제
* HP 회복
* 처형
* 제거

---

## 20. Ghost 상태

Ghost는 상태 기반으로 관리한다.

```text
Normal
Sealed
Executable
Executed
```

### Normal

일반 전투 상태.

가능 행동:

* 이동
* 플레이어 감지
* 추적
* 공격
* 피격

HP가 0 이하가 되면 Sealed로 전환된다.

### Sealed

자동 봉인 상태.

진입 조건:

```text
Ghost HP <= 0
```

Sealed 상태에서는:

* 이동 불가
* 플레이어 추적 불가
* 공격 불가
* Ghost AI 정지
* 원거리 공격 데미지 무효
* F 근거리 처형 무효
* 물리적인 장애물로 남음

Sealed 상태는 **3초 동안 유지**된다.

3초가 지나면 Executable 상태로 전환된다.

### Executable

처형 가능 상태.

Executable 상태에서는:

* Ghost AI 정지
* 이동 불가
* 공격 불가
* 원거리 공격 무효
* F 근거리 처형 가능

Executable 상태는 최대 **5초** 동안 유지된다.

플레이어가 5초 안에 처형하지 못하면 봉인이 풀린다.

### Executed

처형 완료 상태.

* 모든 행동 정지
* 추가 피격 불가
* 추가 처형 불가
* Quest Count 증가
* 처형 VFX 및 SFX
* 이후 Ghost 제거

Ghost가 실제로 죽었다고 판단하는 시점은 HP가 0이 된 순간이 아니라 **Executed 상태에 진입한 순간**이다.

---

## 21. 자동 봉인

기존 기획의 R 수동 봉인 시스템은 사용하지 않는다.

Ghost는 원거리 공격을 받아 HP가 0 이하가 되는 순간 자동으로 봉인된다.

```text
Normal
↓
HP <= 0
↓
EnterSealedState()
```

HP 0을 일반적인 `Die()` 처리로 사용하지 않는다.

HP 0은 사망 조건이 아니라 **봉인 상태 진입 조건**이다.

---

## 22. 봉인 타이머

각 Ghost는 자신의 봉인 타이머를 독립적으로 가지고 있다.

예:

```text
Ghost A 봉인
↓
1초 후 Ghost B 봉인
↓
Ghost A 남은 Sealed 시간 = 2초
Ghost B 남은 Sealed 시간 = 3초
```

두 Ghost의 타이머는 서로 영향을 주지 않는다.

DayManager에서 하나의 공통 봉인 타이머를 관리하지 않는다.

각 `Ghost.cs`가 자신의 다음 정보를 직접 관리한다.

* 현재 State
* HP
* Sealed 남은 시간
* Executable 남은 시간
* 깜빡임 상태

이 구조는 본 프로젝트의 핵심 시스템 중 하나이다.

---

## 23. 처형

### 처형 키

`F`

처형은 근거리 공격이다.

처형 공격 자체는 Ghost에게 일반 데미지를 주지 않는다.

다음 조건을 모두 만족해야만 성공한다.

* Ghost State = Executable
* 처형 범위 안
* 플레이어 전방
* 장애물로 막혀 있지 않음

조건을 만족하는 대상이 여러 마리라면 **플레이어에게 가장 가까운 Ghost 한 마리만** 처형한다.

---

## 24. F 공격 상태별 결과

### Normal Ghost + F

아무 효과 없음.

### Sealed Ghost + F

아무 효과 없음.

### Executable Ghost + F

처형 성공.

### Executed Ghost + F

아무 효과 없음.

---

## 25. 원거리 공격 상태별 결과

### Normal

데미지 적용.

### Sealed

데미지 무효.

### Executable

데미지 무효.

### Executed

상호작용 불가.

따라서 Ghost가 봉인된 이후에는 E 공격을 계속 맞혀도 상태 변화가 발생하지 않는다.

---

## 26. 처형 실패

Executable 상태에서 5초 동안 처형되지 않으면 봉인이 해제된다.

```text
Executable
↓
5초 경과
↓
봉인 해제
↓
HP = Max HP
↓
Normal
↓
Ghost AI 재개
```

봉인 해제 시 Ghost의 HP는 일부만 회복하는 것이 아니라 **최대 HP로 완전히 회복**한다.

따라서 플레이어는 다시 원거리 공격을 통해 Ghost의 HP를 감소시켜야 한다.

---

## 27. 봉인 해제 경고 연출

Executable 상태가 끝나기 **2초 전부터** Ghost가 깜빡이기 시작한다.

남은 시간이 줄어들수록 깜빡이는 속도가 점점 빨라진다.

목적:

* 처형 가능 시간이 얼마 남지 않았다는 정보 제공
* 숫자 타이머 없이 시각적으로 긴장감 전달
* 봉인 해제를 예측 가능하게 만듦

5초가 모두 지나면 봉인이 풀리고 Ghost는 Normal 상태로 돌아간다.

---

## 28. Ghost AI

Ghost의 상태와 AI 행동은 분리한다.

### Ghost.cs

담당:

* HP
* 상태
* 피격
* 봉인
* 타이머
* 봉인 해제
* 처형

### GhostAI.cs

담당:

* 이동
* 플레이어 탐지
* 추적
* 공격

Ghost.cs가 상태의 주체가 된다.

```text
Ghost.Seal()
↓
State = Sealed
↓
GhostAI 정지
```

봉인 해제:

```text
Ghost.Unseal()
↓
State = Normal
↓
GhostAI 재개
```

GhostAI가 자체적으로 봉인 여부를 결정하지 않는다.

---

## 29. Ghost 이동

3D 마을에는 건물, 나무, 울타리 등의 장애물이 존재하므로 Ghost 이동에는 NavMesh 기반 이동을 우선 사용한다.

기본 행동:

```text
Idle / Patrol
↓
Player Detection
↓
추적
↓
Attack Range 도달
↓
공격
```

모든 Ghost가 맵 전체에서 동시에 플레이어를 추적하지 않는다.

Ghost별 Detection Range를 두어 플레이어가 일정 거리 안에 들어오면 행동을 시작한다.

---

## 30. 봉인된 Ghost의 충돌

Sealed 및 Executable Ghost는 장애물처럼 남는다.

Player는 봉인된 Ghost를 자유롭게 통과할 수 없도록 한다.

따라서 전투 중 Ghost를 어디에서 봉인하는지도 플레이에 영향을 줄 수 있다.

단, 필수 이동 경로가 완전히 막히는 상황을 방지하기 위해 다음 위치에서는 Ghost가 장시간 길을 막지 않도록 맵과 Spawn 위치를 설계한다.

* 오두막 입구
* 매우 좁은 통로
* 필수 진행 구간

---

## 31. Ghost 공격 중 봉인

Ghost가 공격 애니메이션이나 공격 Coroutine을 실행하는 중 HP가 0이 되어 봉인될 수 있다.

봉인에 들어가는 순간:

* 진행 중인 공격 취소
* 공격 판정 비활성화
* AI 정지
* 이동 정지

처리를 즉시 수행한다.

AI만 멈추고 이미 실행 중인 공격이 Player에게 데미지를 주는 상황을 방지한다.

---

## 32. GhostSpawner

Stage 1에서는 지나치게 복잡한 무한 스폰 시스템을 사용하지 않는다.

각 Day가 밤으로 전환될 때 해당 Day의 목표 수만큼 Ghost를 한 번 생성한다.

```text
Day 1 Night Start
↓
Day 1 Ghost Spawn

Day 2 Night Start
↓
Day 2 Ghost Spawn
```

한 밤 동안 추가 랜덤 Respawn은 발생하지 않는다.

---

## 33. Spawn 방식

완전 랜덤 위치보다는 미리 배치한 SpawnPoint 후보를 사용한다.

```text
GhostSpawner
├─ SpawnPoint_01
├─ SpawnPoint_02
├─ SpawnPoint_03
├─ SpawnPoint_04
├─ SpawnPoint_05
└─ ...
```

Day마다 사용할 SpawnPoint를 선택한다.

이를 통해 Ghost가 이상한 위치나 플레이 불가능한 위치에 생성되는 문제를 방지한다.

---

## 34. Ghost 중복 생성 방지

하루의 밤이 시작될 때 Ghost 생성은 한 번만 실행한다.

Night Start 이벤트가 중복 호출되어 같은 Day의 Ghost가 여러 번 생성되지 않도록 상태를 관리한다.

예:

```text
nightStarted = true
```

상태에서는 Spawn 함수를 다시 실행하지 않는다.

새 Day가 시작될 때 초기화한다.

---

## 35. 퀘스트 시스템

밤이 시작되기 전 화면 오른쪽에 오늘 처치해야 하는 Ghost 수를 표시한다.

예:

> 오늘 처치해야 하는 귀신
> 0 / 5

퀘스트 목표 수와 실제 생성되는 Ghost 수는 동일하게 한다.

```text
Target Count = 5
↓
Spawn Ghost = 5
```

---

## 36. Quest Count 증가 조건

Ghost HP가 0이 되어도 Quest Count는 증가하지 않는다.

Sealed 상태에서도 증가하지 않는다.

Executable 상태에서도 증가하지 않는다.

오직 **F 처형에 성공하여 Executed 상태가 되었을 때만** 증가한다.

```text
HP 0
→ 0 / 5

Sealed
→ 0 / 5

Executable
→ 0 / 5

Executed
→ 1 / 5
```

---

## 37. 오두막 입장 조건

오두막 입장 조건은 현재 Scene에 Ghost GameObject가 존재하는지를 직접 검사하는 방식으로 만들지 않는다.

DayManager의 Quest 진행 상태를 기준으로 한다.

```text
executedCount >= targetCount
```

이면 입장 가능.

그렇지 않으면 입장 불가.

이를 통해 처형 VFX 때문에 Ghost GameObject가 잠시 남아 있어도 정상적으로 하루를 종료할 수 있다.

---

## 38. DayManager

Stage 1 진행의 중심 관리 시스템이다.

주요 역할:

* 현재 Day
* 현재 아침/밤 상태
* 오늘 목표 Ghost 수
* 현재 처형 Ghost 수
* Night 시작 여부
* GhostSpawner 호출
* Quest 완료 판단
* 오두막 입장 가능 여부
* Day 종료
* 다음 Day 시작
* HP 회복
* 자동 저장
* Day 3 Stage Clear

Ghost 개별 HP나 봉인 타이머는 관리하지 않는다.

---

## 39. 저장 시스템

저장은 매일 아침 자동으로 진행된다.

저장 데이터는 필요한 정보만 최소한으로 관리한다.

예:

```text
Current Stage
Current Day
Stage1Cleared
```

밤 전투 상태는 저장하지 않는다.

저장하지 않는 데이터 예시:

* 현재 Ghost HP
* Ghost 위치
* Ghost 봉인 상태
* Ghost 봉인 남은 시간
* 현재 밤 처형 수

---

## 40. 플레이어 사망

밤 전투 중 Player가 사망하면 해당 밤의 모든 진행을 초기화한다.

```text
Day 2 아침 자동 저장
↓
Day 2 밤
↓
Ghost 3마리 처형
↓
Player 사망
↓
Game Over
↓
재시작
↓
Day 2 아침부터 다시 시작
```

밤의 Ghost 상태 및 퀘스트 진행 상태는 모두 초기화한다.

---

## 41. Day 시작 Player HP

새로운 아침이 시작되면 Player HP를 최대 HP까지 완전히 회복한다.

```text
Day 종료
↓
오두막 취침
↓
다음 날 아침
↓
Player HP = Max HP
```

따라서 각 Day는 하나의 독립적인 전투 구간으로 작동한다.

---

## 42. 자동 저장 안내

플레이어가 저장 규칙을 이해할 수 있도록 초반에 안내한다.

예:

> 진행 상황은 매일 아침 자동 저장됩니다.

밤 중 게임 종료 또는 사망 시 해당 Day의 아침부터 다시 시작한다.

---

## 43. 상호작용 시스템

Space는 공통 상호작용 키로 사용한다.

사용 예:

* 편지 넘기기
* 차량 탑승
* 오두막 문
* 기타 환경 상호작용

각 오브젝트에서 별도로 Space 입력을 중복 처리하기보다는 공통 Interaction 구조를 사용한다.

플레이어가 현재 상호작용 가능한 대상 하나를 판단하여 해당 대상에 Space 입력을 전달한다.

---

## 44. 화면 전환 중 입력 방지

차량 이동, 오두막 입장, 밤 전환 등 화면 전환 중에는 입력 연타로 이벤트가 여러 번 실행되지 않도록 한다.

예:

```text
isTransitioning = true
```

상태에서는 추가 Space 입력을 무시한다.

전환 완료 후 다시 입력을 허용한다.

---

## 45. 게임 방법 UI

오두막에 처음 도착하면 게임 방법 UI가 자동으로 천천히 나타난다.

첫 설명 이후에는 TAB으로 열고 닫을 수 있다.

게임 방법 UI가 열려 있는 동안 게임을 일시정지한다.

일시정지 중:

* 플레이어 이동 정지
* E 공격 불가
* F 처형 불가
* Space 상호작용 불가
* Ghost AI 정지
* Ghost 공격 정지
* Sealed 타이머 정지
* Executable 타이머 정지

게임 시간이 전체적으로 멈춘다.

---

## 46. ESC 일시정지

ESC를 누르면 Pause Menu를 연다.

TAB 게임방법 UI와 ESC Pause Menu가 동시에 활성화되지 않도록 UI 상태를 관리한다.

Pause 중 게임 로직과 Ghost 상태 타이머 역시 정지한다.

---

## 47. UI 구성

### 기본 HUD

왼쪽 상단:

* Day N
* Player HP

오른쪽:

* 오늘의 귀신 목표
* 처형 진행 수

예:

> 오늘 처치해야 하는 귀신
> 3 / 5

### Ghost 피드백

* 봉인 상태 VFX
* Executable 상태 VFX
* 처형 가능 표시
* 봉인 해제 직전 깜빡임

### 기타

* Interaction 안내
* Dialogue UI
* Tutorial UI
* Pause UI
* Game Over UI
* Stage Clear UI

마우스로 직접 조준하는 게임이 아니므로 일반 FPS 스타일 Crosshair는 필수 요소로 사용하지 않는다.

---

## 48. 예상 주요 스크립트

### PlayerController.cs

* WASD 이동
* 카메라 기준 이동 방향 계산
* 이동 방향 바라보기
* 마지막 방향 유지
* 대쉬

### PlayerCombat.cs

* E 원거리 공격
* F 처형
* 처형 대상 탐색
* 가장 가까운 Executable Ghost 판정

### PlayerHealth.cs

* Max HP
* Current HP
* 피격
* 사망
* HP 회복

### CameraController.cs

* Player CameraTarget 추적
* Mouse Camera Rotation
* Player Rotation과 독립적인 회전

### MagicProjectile.cs

* 이동
* Ghost 충돌
* Damage
* Life Time
* 자동 제거

### Ghost.cs

* HP
* GhostState
* 피격
* 자동 봉인
* Sealed 타이머
* Executable 타이머
* 깜빡임
* 봉인 해제
* HP 전체 회복
* 처형
* Executed 처리

### GhostAI.cs

* Player 탐지
* NavMesh 이동
* 추적
* 공격
* AI 활성 / 비활성

### GhostSpawner.cs

* Ghost Prefab 관리
* SpawnPoint 관리
* Day별 Ghost 생성
* 중복 생성 방지

### DayManager.cs

* Current Day
* Day 상태
* Quest
* Night 시작
* GhostSpawner 호출
* Day 종료
* HP 회복
* 자동 저장
* Stage Clear

### SaveManager.cs

* Save
* Load
* New Game 초기화
* Continue
* Stage Clear 저장

### InteractionController.cs

* Space 입력
* 현재 상호작용 대상 탐색
* Vehicle / Door 등 Interaction 호출

### TutorialUI.cs

* 최초 Tutorial 자동 표시
* TAB Open / Close
* Pause 처리

---

## 49. 추천 Unity Hierarchy

```text
Main Scene

Player
├─ Model
├─ ProjectileSpawnPoint
├─ CameraTarget
└─ ExecutionCheck

CameraRig
└─ Main Camera

DayManager

GhostSpawner
├─ SpawnPoint_01
├─ SpawnPoint_02
├─ SpawnPoint_03
├─ SpawnPoint_04
└─ ...

Environment
├─ Cabin
├─ Village
├─ Road
├─ Ground
├─ Buildings
├─ Trees
└─ Props

Canvas
├─ HUD
│  ├─ PlayerHP
│  ├─ DayText
│  └─ QuestUI
├─ InteractionUI
├─ DialogueUI
├─ TutorialUI
├─ PauseUI
├─ GameOverUI
└─ StageClearUI
```

---

## 50. 개발 순서

### Step 1. Unity 프로젝트 기본 세팅

* Unity 3D 프로젝트 생성
* Git / GitHub 설정
* 테스트 Ground 생성
* 임시 Player 생성

### Step 2. Player 이동

* Rigidbody 또는 CharacterController 결정
* WASD 입력
* 카메라 기준 이동
* 이동 방향 회전
* 마지막 방향 유지

### Step 3. Camera

* CameraRig 구성
* Player 추적
* Mouse 회전
* Player Rotation과 독립 처리

### Step 4. 대쉬

* 입력
* 속도 증가
* 지속 시간
* 쿨타임
* 무적 없음

### Step 5. 원거리 공격

* Projectile Prefab
* ProjectileSpawnPoint
* E 입력
* Player.forward 발사
* 속도
* 충돌
* Life Time

### Step 6. 테스트 Ghost

* Ghost Prefab
* HP
* 피격
* Collider

### Step 7. 자동 봉인

* HP <= 0
* Sealed 상태
* AI 정지
* 공격 정지
* 3초 타이머

### Step 8. Executable

* Sealed 3초 완료
* Executable 상태
* 5초 타이머
* 마지막 2초 깜빡임

### Step 9. F 처형

* 전방 근거리 판정
* 가장 가까운 Ghost
* 벽 차단 검사
* Executed 상태
* Ghost 제거

### Step 10. 봉인 해제

* 5초 처형 실패
* HP Max 회복
* Normal 복귀
* AI 재개

### Step 11. Ghost AI

* NavMesh
* Detection Range
* 추적
* 공격

### Step 12. 다수 Ghost 테스트

* Ghost별 독립 Sealed Timer
* Ghost별 독립 Executable Timer
* 여러 Ghost 동시 전투
* 처형 우선순위

### Step 13. DayManager

* Day 1~3
* 아침 / 밤
* Quest
* 하루 종료

### Step 14. GhostSpawner

* SpawnPoint
* Day별 Spawn
* 중복 생성 방지

### Step 15. 오두막

* 밖으로 이동
* Night 전환
* Quest 완료 전 입장 제한
* Day 종료

### Step 16. Save 시스템

* 아침 자동 저장
* Continue
* New Game 경고
* Game Over 재시작
* Stage Clear 저장

### Step 17. Intro

* 혈서 편지
* 주인공 독백
* 차량 Interaction
* Fade
* 차량 SFX
* 오두막 이동

### Step 18. UI

* Player HP
* Day
* Quest
* Interaction
* Tutorial
* Pause
* Game Over
* Stage Clear

### Step 19. 실제 에셋 적용

* 산골 마을
* Player 모델
* Ghost 모델
* 애니메이션
* 환경 에셋

### Step 20. 연출

* Sealed VFX
* Executable VFX
* 깜빡임
* Execution VFX
* Sound
* Lighting
* Day / Night Fade

### Step 21. 밸런싱

* Ghost HP
* Projectile Damage
* Ghost 속도
* Detection Range
* Execution Range
* Sealed 3초 테스트
* Executable 5초 테스트
* Day별 Ghost 수

---

## 51. 포트폴리오 핵심 구현 포인트

### 1. Camera와 Player Rotation 분리

플레이어의 이동 방향과 카메라의 화면 회전을 독립적으로 관리한다.

캐릭터가 회전하더라도 카메라는 자동으로 따라 회전하지 않고 Mouse 입력에 의해서만 화면 방향을 변경한다.

### 2. Camera 기준 WASD 이동

카메라가 어느 방향을 보고 있더라도 WASD 입력이 현재 화면 기준 방향으로 자연스럽게 작동하도록 이동 벡터를 변환한다.

### 3. Player.forward 기반 공격

카메라의 Forward가 아닌 실제 Player.forward를 이용해 마법 투사체의 발사 방향을 계산한다.

### 4. HP 0을 사망이 아닌 상태 전환으로 사용

일반적인 몬스터 시스템과 달리 HP가 0이 되더라도 Ghost를 제거하지 않는다.

HP 0을 Sealed 상태 진입 조건으로 사용한다.

```text
HP 0
≠ Death

HP 0
= Seal Start
```

### 5. 상태 기반 Ghost 시스템

Ghost를 다음 상태로 관리한다.

```text
Normal
→ Sealed
→ Executable
→ Executed
```

처형 실패 시:

```text
Executable
→ Normal
```

로 복귀한다.

### 6. Ghost별 독립 상태 타이머

각 Ghost가 자신만의 Sealed 및 Executable Timer를 관리한다.

다수의 Ghost가 서로 다른 시점에 봉인되더라도 각각 독립적으로 상태가 전환된다.

### 7. 봉인과 AI 연동

Ghost가 Sealed 상태에 진입하면 NavMesh 이동만 정지하는 것이 아니라 다음 행동 전체를 중단한다.

* 이동
* 추적
* 공격
* 실행 중인 공격 판정

봉인이 풀리면 AI를 정상적으로 재개한다.

### 8. 제한 시간 기반 근거리 처형

원거리 공격으로 Ghost를 약화시키고, 봉인 이후 제한 시간 안에 직접 접근해야 처형할 수 있다.

이를 통해 다음 요소를 하나의 전투 흐름으로 연결한다.

* 원거리 전투
* 거리 조절
* 위험한 근거리 접근

### 9. Day 기반 게임 진행

Stage 1을 단순한 몬스터 전멸 Scene으로 구성하지 않고 다음과 같은 반복 구조로 관리한다.

```text
아침
↓
의뢰
↓
밤 전투
↓
오두막 귀환
↓
다음 날
```

### 10. 체크포인트형 저장

매일 아침을 하나의 체크포인트로 사용한다.

밤의 모든 Runtime 전투 상태를 저장하지 않고 Day 단위로 진행을 관리하여 저장 시스템을 단순화한다.

---

## 52. 프로젝트 목표

본 프로젝트는 3D 모델링 자체보다 Unity와 C#을 이용한 게임 시스템 구현 능력을 보여주는 것을 목표로 한다.

Asset Store 및 외부 에셋을 활용하여 그래픽 제작 부담을 줄이고 다음 시스템의 완성도에 집중한다.

* 3D Player Controller
* Mouse Camera
* Camera 기준 이동
* Projectile Combat
* NavMesh Ghost AI
* 상태 기반 Ghost
* 자동 봉인
* 제한 시간 처형
* Ghost별 독립 타이머
* Day 진행 시스템
* Quest 시스템
* Save / Continue
* UI
* VFX / Sound 연출

최종 목표는 단순한 기능 테스트 프로젝트가 아니라,

**혈서 의뢰 확인 → 산골 마을 이동 → 3일간 귀신 퇴치 → Stage 1 완료까지 실제로 플레이 가능한 3D 액션 포트폴리오 게임을 완성하는 것**

이다.
