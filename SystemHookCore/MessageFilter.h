#pragma once

const int messageLimit = 100;

class MessageFilter
{
private:
	int count;
	int excludedMessages[messageLimit];

public:
	MessageFilter()
	{
		count = 0;
	}

	bool AddMessage(int message)
	{
		if (IsFiltered(message))
		{
			return true;
		}

		if (count >= messageLimit)
		{
			return false;
		}

		excludedMessages[count] = message;
		count++;

		return true;
	}

	void Clear()
	{
		count = 0;
	}

	bool IsFiltered(int message)
	{
		for (int i=0; i < count; i++)
		{
			if (excludedMessages[i] == message)
			{
				return true;
			}
		}

		return false;
	}
};
