class Monkey:
    def __init__(self, x, y, team):
        self.x = x
        self.y = y
        self.team = team
        self.level = 1
        self.xp = 0
        self.max_health = 100 + (self.level * 20)
        self.health = self.max_health
        self.attack_DAMAGE = 10 + (self.level * 2)
        
    def gain_xp(self, amount):
        self.xp += amount
        if self.xp >= self.level * 100:
            self.level_up()
            
    def level_up(self):
        self.level += 1
        self.xp = 0
        self.max_health = 100 + (self.level * 20)
        self.health = self.max_health
        self.attack_DAMAGE = 10 + (self.level * 2)
        print(f"Unit leveled up to {self.level}!")
