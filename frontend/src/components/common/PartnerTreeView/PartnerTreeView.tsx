import { useState } from 'react';
import { ChevronDown, ChevronRight, User, Users, Building2 } from 'lucide-react';
import { PartnerTree, PartnerTreeNode } from '@/types';

interface PartnerTreeViewProps {
  tree: PartnerTree;
}

interface PartnerNodeViewProps {
  node: PartnerTreeNode;
  depth?: number;
}

function PartnerNodeView({ node, depth = 0 }: PartnerNodeViewProps) {
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
          <div>
            <span className="font-medium text-gray-900">{node.name}</span>
            <span className="text-sm text-gray-500 ml-2">{node.email || node.phoneNumber}</span>
          </div>
          
          {getLevelBadge(node.level)}

          {node.stats.totalDescendants > 0 && (
            <div className="flex items-center gap-1.5 text-sm text-gray-600">
              <Users className="w-4 h-4" />
              <span>{node.stats.totalDescendants} recomendado{node.stats.totalDescendants !== 1 ? 's' : ''}</span>
            </div>
          )}
        </div>
      </div>

      {/* Children */}
      {hasChildren && isExpanded && (
        <div className="mt-2 ml-6 space-y-2 border-l-2 border-gray-200 pl-4">
          {node.children.map((child) => (
            <PartnerNodeView key={child.id} node={child} depth={depth + 1} />
          ))}
        </div>
      )}
    </div>
  );
}

export function PartnerTreeView({ tree }: PartnerTreeViewProps) {
  return (
    <div className="space-y-4">
      {/* Vetor Info */}
      <div className="flex items-center gap-3 p-4 rounded-lg border-2 border-blue-300 bg-blue-50">
        <div className="flex items-center justify-center w-10 h-10 rounded-full bg-blue-200">
          <Building2 className="w-5 h-5 text-blue-700" />
        </div>
        <div>
          <h3 className="font-semibold text-lg text-gray-900">{tree.vetor.name}</h3>
          <p className="text-sm text-gray-600">{tree.vetor.email}</p>
        </div>
      </div>

      {/* Root Partners */}
      {tree.rootPartners.length > 0 ? (
        <div className="space-y-2">
          {tree.rootPartners.map((partner) => (
            <PartnerNodeView key={partner.id} node={partner} depth={0} />
          ))}
        </div>
      ) : (
        <div className="text-center py-8 text-gray-500">
          Nenhum parceiro encontrado
        </div>
      )}

      {/* Orphan Partners */}
      {tree.orphanPartners.length > 0 && (
        <div className="mt-6">
          <h4 className="text-sm font-semibold text-gray-700 mb-2">Parceiros Órfãos</h4>
          <div className="space-y-2">
            {tree.orphanPartners.map((partner) => (
              <PartnerNodeView key={partner.id} node={partner} depth={0} />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
