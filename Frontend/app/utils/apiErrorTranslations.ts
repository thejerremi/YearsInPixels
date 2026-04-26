type Translate = (key: string, params?: Record<string, string | number>) => string

const apiErrorKeyByMessage: Record<string, string> = {
  'Username is required': 'apiErrors.usernameRequired',
  'Username must be at least 3 characters long': 'apiErrors.usernameMin',
  'Username cannot exceed 20 characters': 'apiErrors.usernameMax',
  'Password is required': 'apiErrors.passwordRequired',
  'Password must be at least 6 characters long': 'apiErrors.passwordMin',
  'Date is required.': 'apiErrors.dateRequired',
  'Mood value must be between 0 and 100.': 'apiErrors.moodRange',
  'You cannot record a pixel for a future date.': 'apiErrors.futureDate',
  'Validation failed': 'apiErrors.validationFailed',
  'Internal Server Error. Please try again later.': 'apiErrors.internalServer',
  'Username already exists.': 'apiErrors.usernameExists',
  'Invalid username or password.': 'apiErrors.invalidCredentials',
  'Invalid refresh token.': 'apiErrors.invalidRefreshToken',
  'You are authenticated!': 'apiErrors.authenticated',
  'No memories found yet.': 'apiErrors.noMemories',
  'Pixel has been saved!': 'apiErrors.pixelSaved',
  'Network Error': 'apiErrors.network'
}

const maxLengthPatterns = [
  {
    regex: /^The day word is too long \(max (?<max>\d+|\{MaxLength\}) characters\)\.$/,
    key: 'apiErrors.dayWordMax'
  },
  {
    regex: /^Your note exceeds the maximum length of (?<max>\d+|\{MaxLength\}) characters\.$/,
    key: 'apiErrors.journalMax'
  }
]

export const translateApiMessage = (message: string, t: Translate) => {
  const directKey = apiErrorKeyByMessage[message]

  if (directKey) {
    return t(directKey)
  }

  for (const pattern of maxLengthPatterns) {
    const match = message.match(pattern.regex)
    const max = match?.groups?.max

    if (max) {
      const fallbackMax = pattern.key === 'apiErrors.dayWordMax' ? 30 : 2000

      return t(pattern.key, { max: max === '{MaxLength}' ? fallbackMax : max })
    }
  }

  return message
}

export const translateApiMessages = (messages: string[], t: Translate) => messages.map(message => translateApiMessage(message, t))
