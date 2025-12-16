import { useState } from 'react';
import { ChevronDown, ChevronRight, User, Users } from 'lucide-react';
import { PartnerTree } from '@/types';

interface PartnerTreeViewProps {
  node: PartnerTree;
  depth?: number;
}

export function PartnerTreeView({ node, depth = 0 }: PartnerTreeViewProps) {
  const [isExpanded, setIsExpanded] = useState(depth < 2); // Auto-expand first 2 levels

  const hasChildren = node.children && node.children.length > 0;
  const indentationClass = depth > 0 ? `ml-${depth * 6}` : '';

  const getLevelBadge = (level: number) => {
    const label = level > 3 ? '3+' : level.toString();
    switch (level) {
      case 1:
        return <span className="inline-flex items-center rounded-full px-3 py-1 text-sm font-medium bg-blue-100 text-blue-800">Nível {label}</span>;
      case 2:
        return <span className="inline-flex items-center rounded-full px-3 py-1 text-sm font-medium bg-green-100 text-green-800">Nível {label}</span>;
      case 3:
        return <span className="inline-flex items-center rounded-full px-3 py-1 text-sm font-medium bg-purple-100 text-purple-800">Nível {label}</span>;
      default:
        return <span className="inline-flex items-center rounded-full px-3 py-1 text-sm font-medium bg-gray-100 text-gray-800">Nível {label}</span>;
    }
  };

  return (
    <div className={`${indentationClass}`}>
      {/* Node */}
      <div
        className={`flex items-center gap-3 p-3 rounded-lg border transition-colors ${
          depth === 0
            ? 'bg-gray-50 border-gray-300'
            : 'bg-white border-gray-200 hover:bg-gray-50'
        }`}
      >
        {/* Expand/Collapse Button */}
        <button
          onClick={() => setIsExpanded(!isExpanded)}
          className={`flex items-center justify-center w-6 h-6 rounded transition-colors ${
            hasChildren
              ? 'hover:bg-gray-200 cursor-pointer'
              : 'invisible'
          }`}
          disabled={!hasChildren}
          aria-label={isExpanded ? 'Colapsar' : 'Expandir'}
        >
          {hasChildren && (
            isExpanded ? (
              <ChevronDown className="w-4 h-4 text-gray-600" />
            ) : (
              <ChevronRight className="w-4 h-4 text-gray-600" />
            )
          )}
        </button>

        {/* Icon */}
        <div className="flex items-center justify-center w-8 h-8 rounded-full bg-gray-100">
          <User className="w-4 h-4 text-gray-600" />
        </div>

        {/* Info */}
        <div className="flex-1 flex items-center gap-3">
          <span className="font-medium text-gray-900">{node.name}</span>
          
          {getLevelBadge(node.level)}

          {node.totalRecommended > 0 && (
            <div className="flex items-center gap-1.5 text-sm text-gray-600">
              <Users className="w-4 h-4" />
              <span>{node.totalRecommended} recomendado{node.totalRecommended !== 1 ? 's' : ''}</span>
            </div>
          )}
        </div>
      </div>

      {/* Children */}
      {hasChildren && isExpanded && (
        <div className="mt-2 ml-6 space-y-2 border-l-2 border-gray-200 pl-4">
          {node.children.map((child) => (
            <PartnerTreeView key={child.id} node={child} depth={depth + 1} />
          ))}
        </div>
      )}
    </div>
  );
}
