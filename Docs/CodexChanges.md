# Codex changes note

Tai lieu nay ghi lai nhung thay doi lon da duoc lam trong project, ly do thay doi, va cach tiep tuc mo rong code hien tai.

## 1. Level khong con giu player that

Huong moi cua project la map/prefab level chi nen giu:

- `LevelContext` tren root map.
- `Player1Spawn` va `Player2Spawn`.
- Cac `Winzone`/`ExitZone`.
- Object cua level nhu grid, coin, trap, block, door.

Player that duoc spawn boi `GameManager`. Cach nay giup:

- Doi skin/player prefab de hon.
- Restart/next level khong can keo tha player vao tung map.
- Moi level chi quan tam den layout va spawn point.

Script lien quan:

- `Assets/Scripts/Extensions/LevelContext.cs`
- `Assets/Scripts/Manager/LevelManager.cs`
- `Assets/Scripts/Manager/GameManager.cs`

## 2. LevelContext

`LevelContext` la component gan tren root cua map. No co nhiem vu:

- Giu reference toi `player1Spawn` va `player2Spawn`.
- Tim va setup cac `Winzone`.
- Tu gan `WinTrigger` vao winzone neu object do chua co.
- Dem coin trong map de setup score.
- Tam ho tro level cu van con player trong map bang cach lay transform cua player cu lam spawn point, sau do disable player cu.

Khi tao map moi, nen gan spawn point ro rang trong Inspector thay vi dua player prefab vao map.

## 3. GameManager

`GameManager` da chuyen sang quan ly player runtime:

- Lan dau load level thi `Instantiate` `player1Prefab` va `player2Prefab`.
- Khi restart/next level thi tai su dung player da co, dua ve spawn point moi bang `ResetForSpawn`.
- Goi `PlayerMoving.SetupPlayer` de movement biet hai player hien tai.
- Goi camera setup lai target theo hai player.
- Co `SetGameplayActive(bool)` de khoa/mo dieu khien khi UI dang mo.

Y tuong quan trong: player la object song dai trong session, level chi thay doi spawn point va level objects.

## 4. LevelManager

`LevelManager` dang lam cac viec:

- Khi scene bat dau, load current level nhung chua cho player di chuyen.
- Khi bam Play hoac chon level, load map tu `Resources/Maps/LevelX`.
- Destroy map cu truoc khi instantiate map moi.
- Lay `LevelContext` tu map moi.
- Setup player, reset win/lose state, va luu current level.
- Kiem tra level co ton tai bang `HasLevel`.

Dieu nay tranh loi next level vuot qua level chua tao.

## 5. Win/Lose

Win logic da duoc doi tu check khoang cach trong movement sang trigger:

- Moi winzone co `WinTrigger`.
- Khi player vao winzone, `WinTrigger` goi `WinLoseManager.NotifyPlayerExited(player)`.
- Player vao winzone se bi set state `Exit` va `SetActive(false)`.
- Khi ca hai player da exit, `WinLoseManager.Win()` hien popup win.

Lose logic:

- Goi `GameManager.SetGameplayActive(false)`.
- Goi `GameManager.RespawnPlayers()` de dua hai player ve spawn point cua map hien tai.
- Hien popup lose va pause game.

## 6. PlayerMoving va input

`PlayerMoving` da them cac lop bao ve input:

- `SetInputEnabled(bool)` de tat/mo dieu khien theo UI.
- `LockInputFor(seconds)` de tranh viec nut UI vua duoc bam dong thoi bi tinh la thao tac di chuyen.
- Bo qua input neu pointer dang nam tren UI.
- Chi cho di chuyen khi player dang `Stand`.
- Neu mot player da exit va bi inactive, player con lai van co the duoc xu ly dung cach cho den khi win.

Day la ly do loi "bam Play nhan vat nhay mot cai" duoc giam: sau khi bam UI, input bi khoa trong mot khoang ngan.

## 7. Player visual va xoay mat

`Player` co `visualRoot` rieng. Khi player di chuyen, code goi:

```csharp
player.FaceDirection(direction);
```

Ta xoay `visualRoot` thay vi xoay root transform cua player de tranh anh huong collider, rigidbody, raycast va logic grid. Root transform nen dai dien cho vi tri gameplay; visual child dai dien cho model/skin.

Neu doi skin ve sau, nen thay model ben trong `visualRoot`, con code movement van giu nguyen.

## 8. Camera va skybox

Camera da duoc zoom ra/cao hon bang orthographic size va auto zoom range de nhan vat/asset vua man hinh dien thoai hon.

Skybox da chuyen sang material `Assets/Materials/Sky.mat` va cac texture:

- `Assets/Sprites/Skybox_Front.png`
- `Assets/Sprites/Skybox_Back.png`
- `Assets/Sprites/Skybox_Left.png`
- `Assets/Sprites/Skybox_Right.png`
- `Assets/Sprites/Skybox_Up.png`
- `Assets/Sprites/Skybox_Down.png`

Luu y: neu anh source khong phai cubemap that su seamless, van co the thay mot vai duong noi nhe. Giai phap tot hon ve lau dai la dung mot panoramic skybox 360 hoac cubemap duoc tao dung chuan.

## 9. Goi y lam nut Home

Trong code hien tai, Home nen co y nghia:

- Dong popup/pause UI.
- Reset win/lose state.
- Tat gameplay input.
- Dua ve homepage/level selected UI.
- Neu muon giu map hien tren nen homepage thi load current level voi `startGameplay = false`.
- Neu muon ve menu sach hon thi destroy current map bang `LevelManager.ReturnToMenu()`.

Huong nen lam cho project nay:

1. Trong `ButtonManager`, tao method `HomeBtn()`.
2. Goi `GameManager.Instance.SetGameplayActive(false)`.
3. Goi `WinLoseManager.Instance.ResetResult()`.
4. Goi `LevelManager.Instance.ReturnToMenu()`.
5. Bat panel homepage.

Vi du logic:

```csharp
public void HomeBtn()
{
    AudioManager.instance.PlayClickSFX();
    HapticManager.LightTaptic();

    Time.timeScale = 1f;
    ClosePanel();

    if (WinLoseManager.Instance != null)
    {
        WinLoseManager.Instance.ResetResult();
    }

    if (GameManager.Instance != null)
    {
        GameManager.Instance.SetGameplayActive(false);
    }

    if (LevelManager.Instance != null)
    {
        LevelManager.Instance.ReturnToMenu();
    }
}
```

Neu homepage cua ban la mot panel trong `uiDatabase`, co the goi `OpenPanel("Home")` hoac `HomePage.SetActive(true)`. Nen tranh goi `StartLevel` trong Home neu muc tieu la ve menu, vi `StartLevel` se load lai map va thuong mo gameplay.

## 10. Viec nen lam tiep

- Tach ro `Homepage`, `Pause`, `Win`, `Lose`, `LevelSelect` thanh enum/string constant de tranh sai ten panel.
- Doi comment bi loi encoding trong mot so file ve ASCII hoac UTF-8 dung chuan.
- Bo cac field cu khong dung nua trong Inspector neu con reference thua.
- Xem lai cac asset bi xoa/asset build trong Git truoc khi lam release that.
- Khi du an on dinh hon, nen them mot scene bootstrap rieng cho manager va UI.
