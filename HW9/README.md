### README

- 本周作业：选第三题，完成一个智能的牧师过河小游戏。

- 下载项目文件夹，打开 Priest-Devil/Assets/start。

- 另外，除了新增智能之外，还修复了基础代码中的一些问题，玩家在游戏对象移动完之后才能移动下一个，保证了每一步执行时游戏对象不会发生错位。

- [展示视频](https://v.youku.com/v_show/id_XMzY3NDI4MTg3Mg==.html?spm=a2h3j.8428770.3416059.1)

### 实验报告

- 首先完成状态机的流程图。因为牧师过河小游戏的解法只有一个正确答案（一条路），因此除了开头可能会有其它状态之外，其它状态要想获得正解都要一条路走到底（其它状态都会Lose）；也就是说，在不Lose的情况下进入的任何非“正解”状态，都能通过上下船来调整P&D的位置，从而回到正解的路上。

- 流程图如下，每个框里面为右岸的状态（P为牧师，D为恶魔，B为船），每条箭头为船移动，箭头上的P&D为船上的角色。

![](/HW9/Assets/Status.png)

- 下面为在原版基础上新增的代码：

1. 首先在用户界面相关代码文件UserGUI.cs，新增一个Next按钮，用来自动走每一步。其中，canCount是为了延长每一步的时间，为了让上下船的动画能执行完整再继续下一步。NextStatus为枚举类型，上船ON，船移动MOVE，和下船OFF；三个步骤循环调用，如果游戏过程中手动移动了游戏对象（鼠标直接点击角色或船），则将三步骤重置为ON（即重新开始）。代码如下：
```CSharp
void OnGUI()
{
    canCount++;
    if (status == 2)
    {
        // ...
    }
    else if (status == 1)
    {
        // ...
    }
    else
    {
        if (GUI.Button(new Rect(Screen.width / 3 - 200, Screen.height / 2, 100, 50), "Next", buttonStyle))
        {
            action.setMovingObj(null);
            if (next == NextStatus.ON)
            {
                if (canCount < 60.0f)
                {
                    return;
                }
                canCount = 0;
                action.nextOnBoat();
                next = NextStatus.MOVE;
            }
            else if (next == NextStatus.MOVE)
            {
                if (canCount < 75.0f)
                {
                    return;
                }
                canCount = 0;
                action.moveBoat();
                next = NextStatus.OFF;
            }
            else if (next == NextStatus.OFF)
            {
                if (canCount < 90.0f)
                {
                    return;
                }
                canCount = 0;
                action.nextOffBoat();
                next = NextStatus.ON;
            }
        }
    }
}
```

2. 接下来实现上船的函数，首先实现一个函数，用以根据当前的状态（右岸），来获得上船的情况（即P&D该怎么上船）。其中BoatStatus为枚举类型，有P, D, PP, DD, PD，决定了船上的角色：
```CSharp
public BoatStatus getNextBS()
    {
        BoatStatus status = new BoatStatus();
        int p = right_coast.getCount(true);
        int d = right_coast.getCount(false);
        if (boat.isRight())
        {
            p += boat.getCount(true);
            d += boat.getCount(false);
        }
        bool isRight = boat.isRight();

        // the right path
        if (p == 3 && d == 3 && isRight)
        {
            status = BoatStatus.PD;
        }
        else if (p == 2 && d == 2 && !isRight)
        {
            status = BoatStatus.P;
        }
        else if (p == 3 && d == 2 && isRight)
        {
            status = BoatStatus.DD;
        }
        else if (p == 3 && d == 0 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 3 && d == 1 && isRight)
        {
            status = BoatStatus.PP;
        }
        else if (p == 1 && d == 1 && !isRight)
        {
            status = BoatStatus.PD;
        }
        else if (p == 2 && d == 2 && isRight)
        {
            status = BoatStatus.PP;
        }
        else if (p == 0 && d == 2 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 0 && d == 3 && isRight)
        {
            status = BoatStatus.DD;
        }
        else if (p == 0 && d == 1 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 0 && d == 2 && isRight)
        {
            status = BoatStatus.DD;
        }
        // the other status
        else if (p == 3 && d == 2 && !isRight)
        {
            status = BoatStatus.D;
        }
        else if (p == 3 && d == 1 && !isRight)
        {
            status = BoatStatus.DD;
        }

        return status;
    }
```

3. 然后先实现一个比较简单的下船函数，让船上的所有人下来即可：
```CSharp
public void nextOffBoat()
{
    for (int i = 0; i < boat.charlist.Length; ++i)
    {
        if (boat.charlist[i] == null)
            continue;
        else
        {
            characterIsClicked(boat.charlist[i]);
        }
    }
}
```

4. 最后实现上船的函数即完成所有代码。根据getNextBS返回的值，来决定让哪边岸的哪些角色上船，上船之前会先调用一次nextOffBoat，让船上的人下来：
```CSharp
public void nextOnBoat()
{
    BoatStatus status = getNextBS();
    nextOffBoat();
    if (status == BoatStatus.P)
    {
        for (int i = 0; i < characters.Length; ++i)
        {
            if (characters[i].getCoastController().isRight() == boat.isRight()
                && characters[i].isPriest())
            {
                characterIsClicked(characters[i]);
                break;
            }
        }
    }
    else if (status == BoatStatus.PP)
    {
        int count = 0;
        for (int i = 0; i < characters.Length; ++i)
        {
            if (characters[i].getCoastController().isRight() == boat.isRight()
                && characters[i].isPriest())
            {
                characterIsClicked(characters[i]);
                ++count;
                if (count == 2)
                    break;
            }
        }
    }
    else if (status == BoatStatus.D)
    {
        for (int i = 0; i < characters.Length; ++i)
        {
            if (characters[i].getCoastController().isRight() == boat.isRight()
                && !characters[i].isPriest())
            {
                characterIsClicked(characters[i]);
                break;
            }
        }
    }
    else if (status == BoatStatus.DD)
    {
        int count = 0;
        for (int i = 0; i < characters.Length; ++i)
        {
            if (characters[i].getCoastController().isRight() == boat.isRight()
                && !characters[i].isPriest())
            {
                characterIsClicked(characters[i]);
                ++count;
                if (count == 2)
                    break;
            }
        }
    }
    else if (status == BoatStatus.PD)
    {
        int count_p = 0;
        int count_d = 0;
        for (int i = 0; i < characters.Length; ++i)
        {
            if (characters[i].getCoastController().isRight() == boat.isRight())
            {
                if (count_p == 0 && characters[i].isPriest())
                {
                    characterIsClicked(characters[i]);
                    count_p++;
                }
                else if (count_d == 0 && !characters[i].isPriest())
                {
                    characterIsClicked(characters[i]);
                    count_d++;
                }
            }
        }
    }
}
```
