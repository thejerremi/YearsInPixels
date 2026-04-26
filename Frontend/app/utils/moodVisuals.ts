export const getMoodColor = (value: number) => {
  const clampedValue = Math.min(100, Math.max(0, value))
  const hue = Math.round((clampedValue / 100) * 132)

  return `hsl(${hue} 78% 48%)`
}
