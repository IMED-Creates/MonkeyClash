def use(self, x, y):
    if self.active and self.game.bananas >= self.cost:
        self.game.bananas -= self.cost
        # Visual feedback
        feedback_text = self.game.font.render(f"-{self.cost}🍌", True, (255, 200, 0))
        self.game.screen.blit(feedback_text, (x, y - 30))
        pygame.display.flip()
        pygame.time.delay(300)  # Brief pause to see the feedback
