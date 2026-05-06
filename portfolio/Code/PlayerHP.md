## 플레이어 체력  
### PlayerCharacter.h  
```cpp
 UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "HP")
    float MaxHP = 100.0f;

    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "HP")
    float CurrentHP;

    void Die();

void TakeDamageFromEnemy(float Damage);
```

### PlayerCharacter.cpp
```cpp
CurrentHP = MaxHP;
void APlayerCharacter::TakeDamageFromEnemy(float Damage)
{
    CurrentHP -= Damage;

    UE_LOG(LogTemp, Warning, TEXT("Player HP: %f"), CurrentHP);

    if (CurrentHP <= 0.0f)
    {
        CurrentHP = 0.0f;
        Die();
    }
}

void APlayerCharacter::Die()
{
    UE_LOG(LogTemp, Warning, TEXT("Player Dead"));
}
```
